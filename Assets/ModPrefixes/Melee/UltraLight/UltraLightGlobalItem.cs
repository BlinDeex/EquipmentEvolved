using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.UltraLight;

public class UltraLightGlobalItem : GlobalItem
{
    public override bool InstancePerEntity => true;

    public override bool? CanAutoReuseItem(Item item, Player player)
    {
        return item.HasPrefix(ModContent.PrefixType<PrefixUltraLight>()) ? true : base.CanAutoReuseItem(item, player);
    }
}