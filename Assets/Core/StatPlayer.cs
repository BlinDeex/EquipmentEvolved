using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.CharmsModule.Augmentations;
using EquipmentEvolved.Assets.CharmsModule.Core;
using EquipmentEvolved.Assets.CharmsModule.Data;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Leggings.Resonant;
using EquipmentEvolved.Assets.ModPrefixes.Melee.Sealed;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Core;

public enum StatSource
{
    Generic,
    AccessoryReforge,
    Charm,
    ArmorSet
}

public class StatPlayer : ModPlayer
{
    public float[] ActiveStats = new float[Enum.GetValues(typeof(PlayerStat)).Length];

    private int invincibilityTicks;

    public bool IsInvincible()
    {
        return invincibilityTicks > 0;
    }

    public void SetInvincibilityTicks(int ticks)
    {
        invincibilityTicks = ticks;
    }

    public override void Initialize()
    {
        ResetEffects();
    }

    public void AddHealingMul(float value)
    {
        HealingMul += value - 1;
    }

    public override void ResetEffects()
    {
        Array.Clear(ActiveStats, 0, ActiveStats.Length);

        HealingMul = 1f;
        MovementSpeedMul = 1f;
        MaxHealthMul = 1f;
        DamageMul = 1f;
        MeleeDamageMul = 1f;
        RangedDamageMul = 1f;
        MagicDamageMul = 1f;
        SummonDamageMul = 1f;
        CharmLuckMul = 1f;
        TrueDamageMul = 1f;
        DamageReductionMul = 1f;
        PickSpeedMul = 1f;
        UseTimeMul = 1f;
        ManaUsageMul = 1f;
        CritDamageMul = 1f;
        CoinDropMul = 1f;
        WingHorizontalAcc = 1f;
        WingHorizontalSpeed = 1f;
        WingVerticalAcc = 1f;
        WingVerticalSpeed = 1f;
        DefenseMul = 1f; 
        HealthCapMul = 1f;

        // Note: Additive stats (like FlatDefense, Aggro, AdditionalMinions) are naturally 0 from Array.Clear

        invincibilityTicks--;
    }
    
    public override bool CanUseItem(Item item)
    {
        if (item.healLife > 0)
        {
            int cappedHealth = (int)(Player.statLifeMax2 * HealthCapMul);
            if (Player.statLife >= cappedHealth)
            {
                return false; 
            }
        }
        return base.CanUseItem(item);
    }

    public float CalculateStatBonus(float baseBonus, StatSource source = StatSource.Generic)
    {
        if (source == StatSource.AccessoryReforge)
        {
            Item leggings = Player.armor[2];
            if (leggings != null && !leggings.IsAir && leggings.HasPrefix(ModContent.PrefixType<PrefixResonant>())) return baseBonus * (1f + PrefixBalance.RESONANT_PREFIX_BOOST);
        }

        return baseBonus;
    }

    public override void UpdateLifeRegen()
    {
        int baseRegen = (int)Regen;
        float leftover = Math.Abs(Regen - baseRegen);

        if (Main.rand.NextFloat() < leftover)
        {
            baseRegen += Math.Sign(Regen); 
        }

        Player.lifeRegen += baseRegen;

        // NEW: Enforce the Health Cap
        if (HealthCapMul < 1f)
        {
            int maxAllowedHealth = (int)(Player.statLifeMax2 * HealthCapMul);
            if (Player.statLife >= maxAllowedHealth)
            {
                Player.statLife = maxAllowedHealth;
                Player.lifeRegen = 0; // Stop vanilla regen from ticking
                Player.lifeRegenTime = 0; // Prevent the natural regen timer from pooling up
            }
        }

        Player.UpdateManaRegen();
    }

    public override void OnHitAnything(float x, float y, Entity victim)
    {
        DropCoins(victim, Player);
        TrueDamage(victim);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        RunLifesteal(target, target.Center);
        RunPercentageLifesteal(target, target.Center, damageDone);
    }

    private void TrueDamage(Entity victim)
    {
        if (TrueDamageFlat != 0) WeaponUtils.InflictTrueDamage(victim as NPC, TrueDamageFlat * TrueDamageMul);

        if (TrueDamagePercentage == 0) return;

        if (victim is not NPC target) return;

        NPC realTarget = target.realLife != -1 ? Main.npc[target.realLife] : target;
        float targetDamage = realTarget.lifeMax * (TrueDamagePercentage * TrueDamageMul);
        WeaponUtils.InflictTrueDamage(realTarget, targetDamage);
    }

    public override void ModifyHurt(ref Player.HurtModifiers modifiers)
    {
        float damageMul = 2 - DamageReductionMul;
        if (damageMul <= 0.1f) damageMul = 0.1f;

        modifiers.FinalDamage *= damageMul;
    }

    public override void PostHurt(Player.HurtInfo info)
    {
        Player.immuneTime += (int)Iframes;
    }

    private void DropCoins(Entity victim, Entity player)
    {
        if (CoinDropOnHit == 0) return;

        CombatUtils.DropCoins(CoinDropOnHit, CoinDropOnHit > 0 ? victim : player);
    }

    public override bool CanBeHitByProjectile(Projectile proj)
    {
        return !IsInvincible();
    }

    public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
    {
        return !IsInvincible();
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.CritDamage += CritDamageMul - 1;
    }

    public override void PostUpdateEquips()
    {
        Item heldItem = Player.inventory[Player.selectedItem];
        if (heldItem != null && !heldItem.IsAir && heldItem.IsWeapon())
        {
            if (heldItem.TryGetGlobalItem(out CharmGlobalItem charmItem))
            {
                CharmDataSnapshot snapshot = charmItem.GetCurrentData();
                if (snapshot.Stats != null && snapshot.Stats.Count > 0) ApplyCharmStats(snapshot.Stats);

                foreach (AugmentationBase snapshotAugmentation in snapshot.Augmentations)
                {
                    snapshotAugmentation?.EnableAugmentation(Player);
                }
            }

            if (heldItem.TryGetGlobalItem(out SealedGlobalItem sealedPrefix))
                if (sealedPrefix.IsSealed && sealedPrefix.IsRevealed && sealedPrefix.Rolls.Count > 0)
                    ApplyCharmStats(sealedPrefix.Rolls);
        }

        Player.wingTimeMax += WingTime;
        Player.GetDamage<GenericDamageClass>() += DamageMul - 1f;
        Player.GetDamage<MeleeDamageClass>() += MeleeDamageMul - 1f;
        Player.GetDamage<RangedDamageClass>() += RangedDamageMul - 1f;
        Player.GetDamage<MagicDamageClass>() += MagicDamageMul - 1f;
        Player.GetDamage<SummonDamageClass>() += SummonDamageMul - 1f;

        Player.statDefense += (int)FlatDefense;
        Player.statDefense.FinalMultiplier *= DefenseMul;
        Player.pickSpeed = CalculateMiningSpeed(Player.pickSpeed, PickSpeedMul);
        Player.maxMinions += (int)AdditionalMinions;
        
        Player.aggro += (int)Aggro;
    }

    public void RunLifesteal(Entity victim, Vector2 hitPos)
    {
        if (OnHitLifesteal == 0) return;

        ApplyHealOrHurt(victim, hitPos, OnHitLifesteal * HealingMul);
    }

    public void RunPercentageLifesteal(Entity victim, Vector2 hitPos, int damageDealt)
    {
        if (DamageLifesteal == 0 || damageDealt <= 0) return;

        ApplyHealOrHurt(victim, hitPos, damageDealt * (DamageLifesteal / 100f) * HealingMul);
    }

    private void ApplyHealOrHurt(Entity victim, Vector2 hitPos, float totalHealAmount)
    {
        bool isNegative = totalHealAmount < 0;
        float lifestealAbs = Math.Abs(totalHealAmount);

        int guaranteedHeals = (int)lifestealAbs;
        float leftOverChance = lifestealAbs - guaranteedHeals;

        if (Main.rand.NextFloat() <= leftOverChance) guaranteedHeals++;

        if (guaranteedHeals == 0) return;

        if (isNegative)
        {
            Player.Hurt(MiscStuff.GetChaoticWeaponDeath(Player.name), guaranteedHeals, 0, armorPenetration: float.MaxValue, dodgeable: false, cooldownCounter: ImmunityCooldownID.General);
            Player.immuneTime = 0;
        }
        else
        {
            // NEW: Respect the Health Cap during Lifesteal
            if (HealthCapMul < 1f)
            {
                int maxAllowedHealth = (int)(Player.statLifeMax2 * HealthCapMul);
                int allowedHeal = maxAllowedHealth - Player.statLife;

                if (allowedHeal <= 0) return;

                guaranteedHeals = Math.Min(guaranteedHeals, allowedHeal);
            }

            CombatUtils.Lifesteal(victim, hitPos, guaranteedHeals, Player);
        }
    }

    private static float CalculateMiningSpeed(float baseSpeed, float speedMul)
    {
        if (speedMul <= 0f) speedMul = 0.01f;

        return baseSpeed / speedMul;
    }

    public override void PostUpdateRunSpeeds()
    {
        Player.accRunSpeed *= MovementSpeedMul;
        Player.maxRunSpeed *= MovementSpeedMul;
        Player.runAcceleration *= MovementSpeedMul;
    }

    public void ApplyCharmStats(List<CharmRoll> rolls)
    {
        foreach (CharmRoll roll in rolls)
        {
            if (roll.Stat == PlayerStat.HealthCapMul)
            {
                // Translates a -0.10f roll into a 0.90f multiplier
                float multiplier = Math.Max(0.01f, 1f + roll.RawStrength);
                ActiveStats[(int)roll.Stat] *= multiplier; 
            }
            else
            {
                ActiveStats[(int)roll.Stat] += roll.RawStrength;
            }
        }
    }

    public override void ModifyWeaponCrit(Item item, ref float crit)
    {
        crit += Crit;
    }

    public override void ModifyManaCost(Item item, ref float reduce, ref float mult)
    {
        mult *= Math.Clamp(ManaUsageMul, 0, 10);
    }

    public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
    {
        healValue = (int)(healValue * HealingMul);

        // NEW: Prevent potions from healing past the cap
        if (HealthCapMul < 1f)
        {
            int maxAllowedHealth = (int)(Player.statLifeMax2 * HealthCapMul);
            int allowedHeal = maxAllowedHealth - Player.statLife;

            if (allowedHeal <= 0)
            {
                healValue = 0;
                return;
            }

            healValue = Math.Min(healValue, allowedHeal);
        }
    }

    public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
    {
        base.ModifyMaxStats(out health, out mana);
        health *= MaxHealthMul;
    }

    private class StatWings : GlobalItem
    {
        public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
        {
            StatPlayer statPlayer = player.GetModPlayer<StatPlayer>();
            speed *= statPlayer.WingHorizontalSpeed;
            acceleration *= statPlayer.WingHorizontalAcc;
        }

        public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier,
            ref float constantAscend)
        {
            StatPlayer statPlayer = player.GetModPlayer<StatPlayer>();
            ascentWhenRising *= statPlayer.WingVerticalAcc;
            maxAscentMultiplier *= statPlayer.WingVerticalSpeed;
        }
    }

    #region stats

    public float AdditionalMinions
    {
        get => ActiveStats[(int)PlayerStat.AdditionalMinions];
        set => ActiveStats[(int)PlayerStat.AdditionalMinions] = value;
    }

    public float CoinDropOnHit
    {
        get => ActiveStats[(int)PlayerStat.CoinDropOnHit];
        set => ActiveStats[(int)PlayerStat.CoinDropOnHit] = value;
    }

    public float Crit
    {
        get => ActiveStats[(int)PlayerStat.Crit];
        set => ActiveStats[(int)PlayerStat.Crit] = value;
    }

    public float FlatDefense
    {
        get => ActiveStats[(int)PlayerStat.FlatDefense];
        set => ActiveStats[(int)PlayerStat.FlatDefense] = value;
    }

    public float TrueDamagePercentage
    {
        get => ActiveStats[(int)PlayerStat.TrueDamagePercentage];
        set => ActiveStats[(int)PlayerStat.TrueDamagePercentage] = value;
    }

    public float TrueDamageFlat
    {
        get => ActiveStats[(int)PlayerStat.TrueDamageFlat];
        set => ActiveStats[(int)PlayerStat.TrueDamageFlat] = value;
    }

    public int WingTime
    {
        get => (int)ActiveStats[(int)PlayerStat.WingTime];
        set => ActiveStats[(int)PlayerStat.WingTime] = value;
    }

    public float Iframes
    {
        get => ActiveStats[(int)PlayerStat.Iframes];
        set => ActiveStats[(int)PlayerStat.Iframes] = value;
    }

    public float Regen
    {
        get => ActiveStats[(int)PlayerStat.Regen];
        set => ActiveStats[(int)PlayerStat.Regen] = value;
    }

    public float Aggro
    {
        get => ActiveStats[(int)PlayerStat.Aggro];
        set => ActiveStats[(int)PlayerStat.Aggro] = value;
    }

    public float CoinDropMul
    {
        get => ActiveStats[(int)PlayerStat.CoinDropMul];
        set => ActiveStats[(int)PlayerStat.CoinDropMul] = value;
    }

    public float WingHorizontalAcc
    {
        get => ActiveStats[(int)PlayerStat.WingHorizontalAcc];
        set => ActiveStats[(int)PlayerStat.WingHorizontalAcc] = value;
    }

    public float WingHorizontalSpeed
    {
        get => ActiveStats[(int)PlayerStat.WingHorizontalSpeed];
        set => ActiveStats[(int)PlayerStat.WingHorizontalSpeed] = value;
    }

    public float WingVerticalAcc
    {
        get => ActiveStats[(int)PlayerStat.WingVerticalAcc];
        set => ActiveStats[(int)PlayerStat.WingVerticalAcc] = value;
    }

    public float WingVerticalSpeed
    {
        get => ActiveStats[(int)PlayerStat.WingVerticalSpeed];
        set => ActiveStats[(int)PlayerStat.WingVerticalSpeed] = value;
    }

    public float DamageLifesteal
    {
        get => ActiveStats[(int)PlayerStat.DamageLifesteal];
        set => ActiveStats[(int)PlayerStat.DamageLifesteal] = value;
    }

    public float OnHitLifesteal
    {
        get => ActiveStats[(int)PlayerStat.LifeSteal];
        set => ActiveStats[(int)PlayerStat.LifeSteal] = value;
    }

    public float HealingMul
    {
        get => ActiveStats[(int)PlayerStat.HealingMul];
        set => ActiveStats[(int)PlayerStat.HealingMul] = value;
    }

    public float CritDamageMul
    {
        get => ActiveStats[(int)PlayerStat.CritDamage];
        set => ActiveStats[(int)PlayerStat.CritDamage] = value;
    }

    public float ManaUsageMul
    {
        get => ActiveStats[(int)PlayerStat.ManaUsage];
        set => ActiveStats[(int)PlayerStat.ManaUsage] = value;
    }

    public float UseTimeMul
    {
        get => ActiveStats[(int)PlayerStat.UseSpeed];
        set => ActiveStats[(int)PlayerStat.UseSpeed] = value;
    }

    public float MovementSpeedMul
    {
        get => ActiveStats[(int)PlayerStat.MoveSpeed];
        set => ActiveStats[(int)PlayerStat.MoveSpeed] = value;
    }

    public float DamageMul
    {
        get => ActiveStats[(int)PlayerStat.Damage];
        set => ActiveStats[(int)PlayerStat.Damage] = value;
    }

    public float PickSpeedMul
    {
        get => ActiveStats[(int)PlayerStat.PickSpeed];
        set => ActiveStats[(int)PlayerStat.PickSpeed] = value;
    }

    public float MeleeDamageMul
    {
        get => ActiveStats[(int)PlayerStat.MeleeDamage];
        set => ActiveStats[(int)PlayerStat.MeleeDamage] = value;
    }

    public float RangedDamageMul
    {
        get => ActiveStats[(int)PlayerStat.RangedDamage];
        set => ActiveStats[(int)PlayerStat.RangedDamage] = value;
    }

    public float MagicDamageMul
    {
        get => ActiveStats[(int)PlayerStat.MagicDamage];
        set => ActiveStats[(int)PlayerStat.MagicDamage] = value;
    }

    public float SummonDamageMul
    {
        get => ActiveStats[(int)PlayerStat.SummonDamage];
        set => ActiveStats[(int)PlayerStat.SummonDamage] = value;
    }

    public float CharmLuckMul
    {
        get => ActiveStats[(int)PlayerStat.CharmLuck];
        set => ActiveStats[(int)PlayerStat.CharmLuck] = value;
    }

    public float TrueDamageMul
    {
        get => ActiveStats[(int)PlayerStat.TrueDamageMul];
        set => ActiveStats[(int)PlayerStat.TrueDamageMul] = value;
    }

    public float DamageReductionMul
    {
        get => ActiveStats[(int)PlayerStat.DamageReduction];
        set => ActiveStats[(int)PlayerStat.DamageReduction] = value;
    }
    
    /// <summary>
    /// Multiplies the player's actual maximum health capacity (the true size of their health bar).
    /// Stacks additively. Use this for standard health buffs that increase the player's total possible life.
    /// </summary>
    public float MaxHealthMul
    {
        get => ActiveStats[(int)PlayerStat.MaxHealthMul];
        set => ActiveStats[(int)PlayerStat.MaxHealthMul] = value > 0 ? value : 0.01f;
    }

    /// <summary>
    /// Restricts the maximum current health the player can regenerate or heal to, 
    /// expressed as a percentage of their true maximum health. 
    /// Perfect for enabling "Low Life" synergies (like Berserker damage buffs).
    /// NOTE: This stat scales multiplicatively to ensure the cap never completely reaches 0.
    /// </summary>
    public float HealthCapMul
    {
        get => ActiveStats[(int)PlayerStat.HealthCapMul];
        set => ActiveStats[(int)PlayerStat.HealthCapMul] = value > 0 ? value : 0.01f;
    }

    public float DefenseMul
    {
        get => ActiveStats[(int)PlayerStat.DefenseMul];
        set => ActiveStats[(int)PlayerStat.DefenseMul] = value > 0 ? value : 0.01f;
    }

    #endregion stats
}