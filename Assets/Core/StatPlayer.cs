using System;
using System.Collections.Generic;
using System.Linq;
using EquipmentEvolved.Assets.CharmsModule.Core;
using EquipmentEvolved.Assets.ModPrefixes.Melee.Sealed;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Core;

/// <summary>
/// The central ledger for the Equipment Evolved player modification system.
/// Tracks, aggregates, and calculates all persistent <see cref="EquipmentStat"/> modifications,
/// manages ticking temporary stats, and registers binary state flags (such as Augmentations or Set Bonuses) applied to the player.
/// </summary>
public class StatPlayer : ModPlayer
{
    private Dictionary<EquipmentStat, Dictionary<StatSource, float>> _activeStats = new();
    
    private List<TemporaryStat> _tempStats = new();
    
    private HashSet<string> _activeFlags = new();

    public override void ResetEffects()
    {
        _activeStats.Clear();
        _activeFlags.Clear();
        
        for (int i = _tempStats.Count - 1; i >= 0; i--)
        {
            _tempStats[i].TimeLeft--;
            
            if (_tempStats[i].TimeLeft <= 0)
            {
                _tempStats.RemoveAt(i);
            }
            else
            {
                AddStat(_tempStats[i].Stat, _tempStats[i].Value, _tempStats[i].Source);
            }
        }
    }
    
    // --- FLAG METHODS ---

    /// <summary>
    /// Registers a string-based binary flag to the player for the current frame.
    /// Useful for cross-mod compatibility or generic states (e.g., "PhantomArmorSet" or "HasCursedDebuff").
    /// </summary>
    /// <param name="flagName">The unique string identifier for the flag.</param>
    public void AddFlag(string flagName)
    {
        _activeFlags.Add(flagName);
    }

    /// <summary>
    /// Registers a type-based binary flag to the player for the current frame.
    /// This is the preferred method for internal mod use (such as Augmentations) to ensure compile-time safety and prevent typos.
    /// </summary>
    /// <typeparam name="T">The class type representing the flag or augmentation.</typeparam>
    public void AddFlag<T>()
    {
        _activeFlags.Add(typeof(T).Name);
    }

    /// <summary>
    /// Checks if a specific string-based flag is currently active on the player.
    /// </summary>
    /// <param name="flagName">The unique string identifier for the flag.</param>
    /// <returns><see langword="true"/> if the flag is active this frame; otherwise, <see langword="false"/>.</returns>
    public bool HasFlag(string flagName)
    {
        return _activeFlags.Contains(flagName);
    }

    /// <summary>
    /// Checks if a specific type-based flag is currently active on the player.
    /// </summary>
    /// <typeparam name="T">The class type representing the flag or augmentation.</typeparam>
    /// <returns><see langword="true"/> if the flag is active this frame; otherwise, <see langword="false"/>.</returns>
    public bool HasFlag<T>()
    {
        return _activeFlags.Contains(typeof(T).Name);
    }
    
    /// <summary>
    /// Calculates the accurately stacked total value of a specific list of temporary stats,
    /// respecting the EquipmentStat's defined StackingMode.
    /// </summary>
    public float CalculateTempStatTotal(EquipmentStat stat, List<TemporaryStat> tempStats)
    {
        if (stat == null || tempStats == null || tempStats.Count == 0) return 0f;
        
        return StackValues(stat.StackingMode, tempStats.Select(t => t.Value));
    }
    
    /// <summary>
    /// Retrieves all currently active temporary stats for a specific EquipmentStat.
    /// Used primarily for Debug UI.
    /// </summary>
    public List<TemporaryStat> GetActiveTempStats(EquipmentStat stat)
    {
        return _tempStats.Where(s => s.Stat == stat).ToList();
    }
    
    /// <summary>
    /// Instantly removes all active temporary stats from the player.
    /// </summary>
    public void ClearTemporaryStats()
    {
        _tempStats.Clear();
    }
    
    /// <summary>
    /// Applies a temporary stat to the player that automatically ticks down.
    /// </summary>
    public void AddTemporaryStat(EquipmentStat stat, float value, int durationTicks, StatReapplicationMode mode, StatSource source = StatSource.Generic)
    {
        if (stat == null || durationTicks <= 0) return;
        
        TemporaryStat existingStat = _tempStats.FirstOrDefault(s => s.Stat == stat && s.Source == source);
         
        if (existingStat != null)
        {
            switch (mode)
            {
                case StatReapplicationMode.Independent:
                    _tempStats.Add(new TemporaryStat { Stat = stat, Value = value, TimeLeft = durationTicks, Source = source });
                    break;
                case StatReapplicationMode.RefreshDuration:
                    existingStat.TimeLeft = Math.Max(existingStat.TimeLeft, durationTicks);
                    break;
                case StatReapplicationMode.StackDuration:
                    existingStat.TimeLeft += durationTicks;
                    break;
                case StatReapplicationMode.StackValue:
                    existingStat.Value += value;
                    break;
                case StatReapplicationMode.Ignore:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }
        else
        {
            _tempStats.Add(new TemporaryStat { Stat = stat, Value = value, TimeLeft = durationTicks, Source = source });
        }
    }

    public void AddStat(EquipmentStat stat, float value, StatSource source = StatSource.Generic)
    {
        if (stat == null) return;
        
        if (!_activeStats.ContainsKey(stat))
            _activeStats[stat] = new Dictionary<StatSource, float>();

        if (!_activeStats[stat].ContainsKey(source))
            _activeStats[stat][source] = 0f;
        
        switch (stat.StackingMode)
        {
            case StatStackingMode.Additive:
                _activeStats[stat][source] += value;
                break;
            case StatStackingMode.Multiplicative:
                _activeStats[stat][source] = (1f + _activeStats[stat][source]) * (1f + value) - 1f;
                break;
            case StatStackingMode.Asymptotic:
                _activeStats[stat][source] = 1f - (1f - _activeStats[stat][source]) * (1f - value);
                break;
            case StatStackingMode.Max:
                _activeStats[stat][source] = Math.Max(_activeStats[stat][source], value);
                break;
            case StatStackingMode.Min:
                if (_activeStats[stat][source] == 0f) _activeStats[stat][source] = value;
                else _activeStats[stat][source] = Math.Min(_activeStats[stat][source], value);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public float GetTotalStat(EquipmentStat stat)
    {
        if (stat == null || !_activeStats.TryGetValue(stat, out Dictionary<StatSource, float> value)) return 0f;
        
        // Pass the dictionary's values directly to the helper
        return StackValues(stat.StackingMode, value.Values);
    }

    public float GetStatFromSource(EquipmentStat stat, StatSource source)
    {
        if (stat == null || !_activeStats.TryGetValue(stat, out Dictionary<StatSource, float> value)) return 0f;
        return value.GetValueOrDefault(source, 0f);
    }

    public void MultiplySourceStats(StatSource source, float multiplier)
    {
        foreach (var statDict in _activeStats.Values)
        {
            if (statDict.ContainsKey(source))
            {
                statDict[source] *= multiplier;
            }
        }
    }

    /// <summary>
    /// Extracts and applies stats from the player's currently held item (Weapons/Tools).
    /// </summary>
    private void HeldItemStats()
    {
        Item heldItem = Player.HeldItem;
        if (heldItem == null || heldItem.IsAir) return;
        
        // NEW: Abort if the item in our hand is an accessory or an armor piece! 
        // This ensures stats are only pulled when holding a weapon/tool.
        if (heldItem.accessory || heldItem.IsArmor()) return;
        
        if (heldItem.TryGetGlobalItem(out CharmGlobalItem charmData))
        {
            charmData.ApplyHeldStats(Player);
        }
        
        if (heldItem.TryGetGlobalItem(out SealedGlobalItem sealedData) && sealedData.IsRevealed)
        {
            ApplyCharmStats(sealedData.Rolls);
        }
    }
    
    /// <summary>
    /// Helper method to mathematically stack a collection of values based on the provided StackingMode.
    /// </summary>
    private float StackValues(StatStackingMode mode, IEnumerable<float> values)
    {
        float total = mode == StatStackingMode.Min ? float.MaxValue : 0f;
        bool hasValues = false;

        foreach (float val in values)
        {
            hasValues = true;
            switch (mode)
            {
                case StatStackingMode.Additive:
                    total += val;
                    break;
                case StatStackingMode.Multiplicative:
                    total = (1f + total) * (1f + val) - 1f;
                    break;
                case StatStackingMode.Asymptotic:
                    total = 1f - (1f - total) * (1f - val);
                    break;
                case StatStackingMode.Max:
                    total = Math.Max(total, val);
                    break;
                case StatStackingMode.Min:
                    total = Math.Min(total, val);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (mode == StatStackingMode.Min && !hasValues) return 0f;

        return total;
    }
    
    public void ApplyCharmStats(List<CharmRoll> rolls)
    {
        foreach (var roll in rolls)
        {
            if (roll.Stat != null && !roll.IsUnloaded)
            {
                AddStat(roll.Stat, roll.Strength, StatSource.Charm);
            }
        }
    }

    public override void UpdateEquips()
    {
        HeldItemStats();
        foreach (var stat in _activeStats.Keys)
            stat.UpdateEquips(Player, GetTotalStat(stat));
    }
    
    public override bool CanUseItem(Item item)
    {
        return _activeStats.Keys.All(stat => stat.CanUseItem(Player, item, GetTotalStat(stat))) && base.CanUseItem(item);
    }

    public override void PostUpdateEquips()
    {
        foreach (var stat in _activeStats.Keys)
            stat.PostUpdateEquips(Player, GetTotalStat(stat));
    }

    public override void UpdateLifeRegen()
    {
        foreach (var stat in _activeStats.Keys)
            stat.UpdateLifeRegen(Player, GetTotalStat(stat));
    }

    public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
    {
        health = StatModifier.Default;
        mana = StatModifier.Default;
        foreach (var stat in _activeStats.Keys)
            stat.ModifyMaxStats(Player, ref health, ref mana, GetTotalStat(stat));
    }

    public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
    {
        foreach (var stat in _activeStats.Keys)
            stat.ModifyWeaponDamage(Player, item, ref damage, GetTotalStat(stat));
    }

    public override void ModifyWeaponCrit(Item item, ref float crit)
    {
        foreach (var stat in _activeStats.Keys)
            stat.ModifyWeaponCrit(Player, item, ref crit, GetTotalStat(stat));
    }

    public override void ModifyWeaponKnockback(Item item, ref StatModifier knockback)
    {
        foreach (var stat in _activeStats.Keys)
            stat.ModifyWeaponKnockback(Player, item, ref knockback, GetTotalStat(stat));
    }

    public override void ModifyManaCost(Item item, ref float reduce, ref float mult)
    {
        foreach (var stat in _activeStats.Keys)
            stat.ModifyManaCost(Player, item, ref reduce, ref mult, GetTotalStat(stat));
    }

    public override float UseSpeedMultiplier(Item item)
    {
        float globalMult = 1f;
        foreach (var stat in _activeStats.Keys)
            globalMult *= stat.UseSpeedMultiplier(Player, item, GetTotalStat(stat));
        return globalMult;
    }
    
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        foreach (var stat in _activeStats.Keys)
            stat.ModifyHitNPC(Player, target, ref modifiers, GetTotalStat(stat));
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        foreach (var stat in _activeStats.Keys)
            stat.OnHitNPC(Player, target, hit, damageDone, GetTotalStat(stat));
    }
    
    public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
    {
        foreach (var stat in _activeStats.Keys)
            stat.ModifyHitNPCWithItem(Player, item, target, ref modifiers, GetTotalStat(stat));
    }

    public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
    {
        foreach (var stat in _activeStats.Keys)
            stat.OnHitNPCWithItem(Player, item, target, hit, damageDone, GetTotalStat(stat));
    }

    public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
    {
        foreach (var stat in _activeStats.Keys)
            stat.ModifyHitNPCWithProj(Player, proj, target, ref modifiers, GetTotalStat(stat));
    }

    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        foreach (var stat in _activeStats.Keys)
            stat.OnHitNPCWithProj(Player, proj, target, hit, damageDone, GetTotalStat(stat));
    }

    public override void ModifyHurt(ref Player.HurtModifiers modifiers)
    {
        foreach (var stat in _activeStats.Keys)
            stat.ModifyHurt(Player, ref modifiers, GetTotalStat(stat));
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        foreach (var stat in _activeStats.Keys)
            stat.OnHurt(Player, info, GetTotalStat(stat));
    }

    public override void PostHurt(Player.HurtInfo info)
    {
        foreach (var stat in _activeStats.Keys)
            stat.PostHurt(Player, info, GetTotalStat(stat));
    }

    public override bool FreeDodge(Player.HurtInfo info)
    {
        bool dodged = false;
        foreach (var stat in _activeStats.Keys)
        {
            if (stat.FreeDodge(Player, info, GetTotalStat(stat)))
                dodged = true;
        }
        return dodged;
    }
    
    public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
    {
        foreach (var stat in _activeStats.Keys)
            stat.GetHealLife(Player, item, quickHeal, ref healValue, GetTotalStat(stat));
    }
    
    public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        foreach (var stat in _activeStats.Keys)
        {
            stat.ModifyShootStats(Player, item, ref position, ref velocity, ref type, ref damage, ref knockback, GetTotalStat(stat));
        }
    }
}