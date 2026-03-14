using System;

namespace EquipmentEvolved.Assets.Core;

[Flags]
public enum StatSource
{
    Generic = 1 << 0,
    Weapon = 1 << 1,
    Headwear = 1 << 2,
    Chestplate = 1 << 3,
    Leggings = 1 << 4,
    Accessory = 1 << 5,
    Charm = 1 << 6,
    
    Armor = Headwear | Chestplate | Leggings,
}