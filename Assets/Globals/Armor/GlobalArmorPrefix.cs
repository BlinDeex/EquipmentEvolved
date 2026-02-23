using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Globals.Armor;

public class GlobalArmorPrefix : GlobalItem
{
    public override void SetDefaults(Item entity)
    {
        entity.accessory = entity.accessory || entity.IsArmor();
    }
}