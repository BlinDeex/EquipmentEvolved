using System;

namespace EquipmentEvolved.Assets.Misc;

[Flags]
public enum SpecializedPrefixType
{
    Empty = 0,
    // Tools
    Pickaxe = 1,
    Axe = 2,
    Hammer = 4,
    
    // Summoner
    MinionWeapon = 8,
    Whip = 16,
    
    // Armor
    Headwear = 32,
    Chestplate = 64,
    Leggings = 128,
    
    // NEW: Standard Weapons
    MeleeWeapon = 256, // Swords, Spears, Yoyos
    RangedWeapon = 512, // Bows, Guns
    MagicWeapon = 1024, // Staffs, Tomes
    
    // Combinations
    AnyArmor = Headwear | Chestplate | Leggings,
    AnyTool = Pickaxe | Axe | Hammer,
    
    // Updated Any
    Any = Pickaxe | Axe | Hammer | MinionWeapon | Whip | Headwear | Chestplate | Leggings | MeleeWeapon | RangedWeapon | MagicWeapon
}

public enum ChallengerOrbType
{
    Green = 0,
    Blue = 1,
    Yellow = 2,
    Red = 3
}

public enum MessageType
{
    ChallengerScore,
    TrueDamageText,
    TimeStop,
    CharmOnKilled,
    SyncPrefixPlayer,
    PerceptiveCritEffect,
    SilentTileKill,
    SyncToolPlayer,
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