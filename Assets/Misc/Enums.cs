using System;

namespace EquipmentEvolved.Assets.Misc;

[Flags]
public enum SpecializedPrefixType
{
    Empty = 0,

    Pickaxe = 1 << 0,
    Axe = 1 << 1,
    Hammer = 1 << 2,

    MinionWeapon = 1 << 3,
    Whip = 1 << 4,

    Headwear = 1 << 5,
    Chestplate = 1 << 6,
    Leggings = 1 << 7,

    MeleeWeapon = 1 << 8,
    RangedWeapon = 1 << 9,
    MagicWeapon = 1 << 10,

    AnyArmor = Headwear | Chestplate | Leggings,
    AnyTool = Pickaxe | Axe | Hammer,
    StandardWeapons = MeleeWeapon | RangedWeapon | MagicWeapon,

    Any = Pickaxe | Axe | Hammer | MinionWeapon | Whip | Headwear | Chestplate | Leggings | MeleeWeapon | RangedWeapon | MagicWeapon
}

public enum MessageType
{
    ChallengerScore,
    TrueDamageText,
    TimeStop,
    CharmOnKilled,
    SyncInvertedModPlayer,
    PerceptiveCritEffect,
    SilentTileKill,
    SyncFortuneModPlayer,
    NegatedText
}

public enum ArmorType
{
    Headwear,
    Chestplate,
    Leggings
}

public enum LogType
{
    Debug,
    Log,
    Warning,
    Error
}