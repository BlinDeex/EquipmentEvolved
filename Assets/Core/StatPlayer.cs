using System;
using System.Collections.Generic;
using System.Linq;
using EquipmentEvolved.Assets.CharmsModule.Core;
using EquipmentEvolved.Assets.ModPrefixes.Melee.Sealed;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Core;

/// <summary>
/// The central ledger for the Equipment Evolved stat system.
/// Tracks, aggregates, and calculates all <see cref="EquipmentStat"/> modifications applied to the player.
/// </summary>
public class StatPlayer : ModPlayer
{
    /// <summary>
    /// A dictionary mapping each active stat to its respective values, separated by their originating <see cref="StatSource"/>.
    /// </summary>
    private Dictionary<EquipmentStat, Dictionary<StatSource, float>> _activeStats = new();

    public override void ResetEffects()
    {
        _activeStats.Clear();
    }

    /// <summary>
    /// Adds a value to a specific stat for a given source.
    /// The value is mathematically combined with existing values from the same source based on the stat's <see cref="StatStackingMode"/>.
    /// </summary>
    /// <param name="stat">The stat to modify.</param>
    /// <param name="value">The value to add to the stat.</param>
    /// <param name="source">The originating source of the stat modification (defaults to <see cref="StatSource.Generic"/>).</param>
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

    /// <summary>
    /// Calculates the total combined value of a specific stat across all active sources.
    /// The total is mathematically aggregated based on the stat's <see cref="StatStackingMode"/>.
    /// </summary>
    /// <param name="stat">The stat to calculate.</param>
    /// <returns>The total aggregated value of the stat, or 0f if the stat is not currently active.</returns>
    public float GetTotalStat(EquipmentStat stat)
    {
        if (stat == null || !_activeStats.ContainsKey(stat)) return 0f;
        
        float total = stat.StackingMode == StatStackingMode.Min ? float.MaxValue : 0f;
        bool hasValues = false;
        
        foreach (float val in _activeStats[stat].Values)
        {
            hasValues = true;
            switch (stat.StackingMode)
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
        
        if (stat.StackingMode == StatStackingMode.Min && !hasValues) return 0f;
        
        return total;
    }

    /// <summary>
    /// Retrieves the raw value of a stat associated exclusively with a specific source.
    /// </summary>
    /// <param name="stat">The stat to retrieve.</param>
    /// <param name="source">The source to check.</param>
    /// <returns>The value of the stat from the given source, or 0f if not found.</returns>
    public float GetStatFromSource(EquipmentStat stat, StatSource source)
    {
        if (stat == null || !_activeStats.ContainsKey(stat)) return 0f;
        return _activeStats[stat].TryGetValue(source, out float val) ? val : 0f;
    }

    /// <summary>
    /// Multiplies all currently active stat values originating from a specific source by a given multiplier.
    /// Useful for set bonuses or buffs that globally amplify certain equipment types.
    /// </summary>
    /// <param name="source">The stat source to amplify.</param>
    /// <param name="multiplier">The multiplier to apply.</param>
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
    /// Extracts and applies stats from the player's currently held item, such as applied charms or sealed modifiers.
    /// </summary>
    private void HeldItemStats()
    {
        Item heldItem = Player.HeldItem;
        if (heldItem == null || heldItem.IsAir) return;
        
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
    /// Applies a list of charm rolls to the player under the <see cref="StatSource.Charm"/> source.
    /// Automatically ignores stats that are null or unloaded.
    /// </summary>
    /// <param name="rolls">The list of charm rolls to apply.</param>
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
        foreach (var stat in _activeStats.Keys)
        {
            if (!stat.CanUseItem(Player, item, GetTotalStat(stat)))
            {
                return false;
            }
        }
    
        return base.CanUseItem(item);
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
}