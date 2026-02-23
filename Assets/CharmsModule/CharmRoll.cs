using System;

namespace EquipmentEvolved.Assets.CharmsModule;

public enum CharmStat
{
    NotInitialized,
    Damage,
    MeleeDamage,
    RangedDamage,
    MagicDamage,
    SummonDamage,
    MoveSpeed,
    WingTime,
    UseSpeed,
    LifeSteal,
    CharmLuck,
    CritDamage,
    ManaUsage,
    HealingMul,
    Regen,
    PickSpeed,
    MaxHealthMul,
    Crit,
    Iframes,
    DamageReduction,
    TrueDamageMul
}



public class CharmRoll(CharmStat stat, float strength)
{
    public CharmStat Stat { get; private set; } = stat;

    public float GetStrength(bool multiply = true)
    {
        return MathF.Round(strength * (multiply ? 100 : 1), 2);
    }

    public float RawStrength => strength;
}