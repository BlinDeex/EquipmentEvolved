namespace EquipmentEvolved.Assets.Core;

public enum PlayerStat
{
    NotInitialized,

    // Core Combat
    Damage,
    MeleeDamage,
    RangedDamage,
    MagicDamage,
    SummonDamage,
    TrueDamageMul,
    TrueDamagePercentage,
    TrueDamageFlat,
    Crit,
    CritDamage,
    UseSpeed,

    // Core Defense & Health
    MaxHealthMul,
    FlatDefense,
    DefenseMul,
    DamageReduction,
    Regen,
    HealingMul,
    LifeSteal,
    DamageLifesteal,
    Iframes,
    CritMul,

    // Mobility
    MoveSpeed,
    WingTime,
    WingHorizontalAcc,
    WingHorizontalSpeed,
    WingVerticalAcc,
    WingVerticalSpeed,

    // Utility & Misc
    ManaUsage,
    PickSpeed,
    CharmLuck,
    AdditionalMinions,
    CoinDropOnHit,
    CoinDropMul,
    Aggro,
    ScaleMul
}