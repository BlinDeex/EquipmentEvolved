using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Headwear.Enraging;

public class EnragingGlobalItem : GlobalItem
{
    public override void UpdateEquip(Item item, Player player)
    {
        if (item.HasPrefix(ModContent.PrefixType<PrefixEnraging>())) player.aggro += PrefixBalance.ENRAGING_AGGRO_INCREASE;
    }
}