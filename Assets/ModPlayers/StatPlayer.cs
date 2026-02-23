using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.CharmsModule;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPlayers;

public class StatPlayer : ModPlayer
{
    public Dictionary<CharmStat, (Func<float> Getter, Action<float> Setter)> StatAccessors { get; }

    public StatPlayer()
    {
        StatAccessors = new Dictionary<CharmStat, (Func<float> Getter, Action<float> Setter)>()
        {
            { CharmStat.LifeSteal, (() => Lifesteal, value => Lifesteal = value) },
            { CharmStat.HealingMul, (() => HealingMul, value => HealingMul = value) },
            { CharmStat.CritDamage, (() => CritDamageMul, value => CritDamageMul = value) },
            { CharmStat.ManaUsage, (() => ManaUsageMul, value => ManaUsageMul = value) },
            { CharmStat.UseSpeed, (() => UseTimeMul, value => UseTimeMul = value) },
            { CharmStat.MoveSpeed, (() => MovementSpeedMul, value => MovementSpeedMul = value) },
            { CharmStat.WingTime, (() => WingTime, value => WingTime = (int)value) },
            { CharmStat.MaxHealthMul, (() => MaxHealthMul, value => MaxHealthMul = value) },
            { CharmStat.Crit, (() => CritMul, value => CritMul = value) },
            { CharmStat.Damage, (() => DamageMul, value => DamageMul = value) },
            { CharmStat.PickSpeed, (() => PickSpeedMul, value => PickSpeedMul = value) },
            { CharmStat.MeleeDamage, (() => MeleeDamageMul, value => MeleeDamageMul = value) },
            { CharmStat.RangedDamage, (() => RangedDamageMul, value => RangedDamageMul = value) },
            { CharmStat.MagicDamage, (() => MagicDamageMul, value => MagicDamageMul = value) },
            { CharmStat.SummonDamage, (() => SummonDamageMul, value => SummonDamageMul = value) },
            { CharmStat.CharmLuck, (() => CharmLuckMul, value => CharmLuckMul = value) },
            { CharmStat.Iframes, (() => Iframes, value => Iframes = value) },
            { CharmStat.TrueDamageMul, (() => TrueDamageMul, value => TrueDamageMul = value) },
            { CharmStat.DamageReduction, (() => DamageReductionMul, value => DamageReductionMul = value) },
        };
    }
    
    private float additionalMinions;
    public float AdditionalMinions 
    {
        get => additionalMinions;
        set
        {
            if ((int)additionalMinions != (int)(additionalMinions + value))
            {
                int newMinions = (int)(additionalMinions + value) - (int)additionalMinions;
                Player.maxMinions += newMinions;
            }

            additionalMinions = value;
        }
    }
    
    public float Lifesteal { get; set; }
    public float HealingMul { get; set; } = 1f;
    public float CritDamageMul { get; set; }
    public float ManaUsageMul { get; set; }
    public float UseTimeMul { get; set; }
    public float CoinDropValue { get; set; }
    public float CoinDropValueMul { get; set; }
    public int Regen { get; set; }
    public float MovementSpeedMul { get; set; } = 1f;
    public int WingTime { get; set; }
    private float maxHealthMul = 1f;
    public float MaxHealthMul
    {
        get => maxHealthMul;
        set => maxHealthMul = value > 0 ? value : 0.01f;
    }

    public float CritMul { get; set; } = 1f;
    public float Crit { get; set; }
    public float DamageMul { get; set; } = 1f;
    public float MeleeDamageMul { get; set; }
    public float RangedDamageMul { get; set; } = 1f;
    public float MagicDamageMul { get; set; } = 1f;
    public float SummonDamageMul { get; set; } = 1f;

    public float CharmLuckMul { get; set; } = 1f;
    
    public float WingHorizontalAcc { get; set; } = 1f;
    public float WingHorizontalSpeed { get; set; } = 1f;
    public float WingVerticalAcc { get; set; } = 1f;
    public float WingVerticalSpeed { get; set; } = 1f;

    public float TrueDamageMul { get; set; } = 1f;
    public float TrueDamagePercentage { get; set; }
    public float TrueDamageFlat { get; set; }

    public float DamageReductionMul { get; set; } = 1f;
    
    public float Iframes { get; set; }

    public float PickSpeedMul { get; set; } = 1f;
    private float defenseMul;

    private int invincibilityTicks;

    public bool IsInvincible() => invincibilityTicks > 0;
    public void SetInvincibilityTicks(int ticks) => invincibilityTicks = ticks;

    public float DefenseMul
    {
        get => defenseMul;
        set => defenseMul = value > 0 ? value : 0.01f;
    }

    public float FlatDefense { get; set; }

    public void AddHealingMul(float value)
    {
        HealingMul += value - 1;
    }

    public override void ResetEffects()
    {
        Player.maxMinions -= (int)AdditionalMinions;
        AdditionalMinions = 0;

        PickSpeedMul = 1f;
        ManaUsageMul = 1f;
        UseTimeMul = 1f;
        CoinDropValue = 0f;
        CoinDropValueMul = 1f;
        Lifesteal = 0f;
        HealingMul = 1f;
        Regen = 0;
        MovementSpeedMul = 1f;
        WingTime = 0;
        MaxHealthMul = 1f;
        Crit = 0f;
        DamageMul = 1f;
        DefenseMul = 1f;
        CritDamageMul = 1f;
        CritMul = 1f;
        MeleeDamageMul = 1f;
        RangedDamageMul = 1f;
        MagicDamageMul = 1f;
        SummonDamageMul = 1f;
        CharmLuckMul = 1f;
        WingHorizontalAcc = 1f;
        WingHorizontalSpeed = 1f;
        WingVerticalAcc = 1f;
        WingVerticalSpeed = 1f;
        FlatDefense = 0f;
        
        TrueDamageMul = 1f;
        TrueDamagePercentage = 0f;
        TrueDamageFlat = 0f;
        DamageReductionMul = 1f;
        Iframes = 0f;

        invincibilityTicks--;
    }
    public override void UpdateLifeRegen()
    {
        Player.lifeRegen += Regen;
        Player.UpdateManaRegen();
    }
    
    

    public override void OnHitAnything(float x, float y, Entity victim)
    {
        RunLifesteal(victim, new Vector2(x, y));
        DropCoins(victim, Player);
        TrueDamage(victim);
    }

    private void TrueDamage(Entity victim)
    {
        if (TrueDamageFlat != 0)
        {
            WeaponUtils.InflictTrueDamage(victim as NPC, TrueDamageFlat * TrueDamageMul);
        }

        if (TrueDamagePercentage == 0) return;
        if (victim is not NPC target) return;
        
        NPC realTarget = target.realLife != -1 ? Main.npc[target.realLife] : target;
        float targetDamage = realTarget.lifeMax * (TrueDamagePercentage * TrueDamageMul);
        WeaponUtils.InflictTrueDamage(realTarget, targetDamage);
    }

    public override void ModifyHurt(ref Player.HurtModifiers modifiers)
    {
        modifiers.FinalDamage *= DamageReductionMul; 
    }

    public override void PostHurt(Player.HurtInfo info)
    {
        Player.immuneTime += (int)Iframes;
    }

    private void DropCoins(Entity victim, Entity player)
    {
        if (CoinDropValue == 0) return; 
        CombatUtils.DropCoins(CoinDropValue, CoinDropValue > 0 ? victim : player);
    }

    public override bool CanBeHitByProjectile(Projectile proj)
    {
        return !IsInvincible();
    }

    public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
    {
        return !IsInvincible();
    }

    private void RunLifesteal(Entity victim, Vector2 hitPos)
    {
        if (Lifesteal == 0) return;
        float currentLifeSteal = Lifesteal * HealingMul;
        bool isNegative = currentLifeSteal < 0;
        float lifestealAbs = Math.Abs(currentLifeSteal);
        int fullLifesteals = (int)lifestealAbs;

        float leftOver = lifestealAbs - fullLifesteals;

        float dice = Main.rand.NextFloat();

        if (dice <= leftOver) fullLifesteals++;

        if (fullLifesteals == 0) return;

        if (isNegative)
        {
            Player.Hurt(MiscStuff.CHAOTIC_WEAPON_DEATH, fullLifesteals, 0, armorPenetration: float.MaxValue,
                dodgeable: false, cooldownCounter: ImmunityCooldownID.General);
            Player.immuneTime = 0;
        }
        else
        {
            CombatUtils.Lifesteal(victim, hitPos, fullLifesteals, Player);
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.CritDamage += CritDamageMul - 1;
    }

    public override void PostUpdateEquips()
    {
        Player.wingTimeMax += WingTime;
        Player.GetAttackSpeed(DamageClass.Generic) *= UseTimeMul;
        Player.GetDamage<GenericDamageClass>() *= DamageMul;
        Player.GetDamage<MeleeDamageClass>() *= MeleeDamageMul;
        Player.GetDamage<RangedDamageClass>() *= RangedDamageMul;
        Player.GetDamage<MagicDamageClass>() *= MagicDamageMul;
        Player.GetDamage<SummonDamageClass>() *= SummonDamageMul;
        Player.statDefense += (int)FlatDefense;
        Player.statDefense.FinalMultiplier *= defenseMul;
        Player.pickSpeed = CalculateMiningSpeed(Player.pickSpeed, PickSpeedMul);
    }
    
    private static float CalculateMiningSpeed(float baseSpeed, float speedMul)
    {
        float finalSpeed = baseSpeed / speedMul;
        if (finalSpeed <= 0) finalSpeed = 0.01f;
        return finalSpeed;
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
            if (StatAccessors.TryGetValue(roll.Stat, out (Func<float> Getter, Action<float> Setter) accessor))
            {
                accessor.Setter(accessor.Getter.Invoke() + roll.RawStrength);
            }
            else
            {
                UtilMethods.LogMessage($"No accessor for {roll.Stat}", LogType.Error);
            }
        }
    }

    public override void ModifyWeaponCrit(Item item, ref float crit)
    {
        crit += Crit;
        crit *= CritMul;
    }

    public override void ModifyManaCost(Item item, ref float reduce, ref float mult)
    {
        mult *= Math.Clamp(ManaUsageMul, 0, 10);
    }

    public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
    {
        healValue = (int)(healValue * HealingMul);
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

        public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling,
            ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            StatPlayer statPlayer = player.GetModPlayer<StatPlayer>();
            ascentWhenRising *= statPlayer.WingVerticalAcc;
            maxAscentMultiplier *= statPlayer.WingVerticalSpeed;
        }
    }
}