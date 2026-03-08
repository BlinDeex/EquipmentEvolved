using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Chestplate.Dashing;

public class DashingGlobalItem : GlobalItem
{
    public override void UpdateEquip(Item item, Player player)
    {
        if (item.HasPrefix(ModContent.PrefixType<PrefixDashing>())) player.GetModPlayer<DashingModPlayer>().IsDashingEquipped = true;
    }
}