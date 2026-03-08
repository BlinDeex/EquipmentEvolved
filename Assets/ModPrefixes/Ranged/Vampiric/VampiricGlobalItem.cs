using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Vampiric;

public class VampiricGlobalItem : GlobalItem
{
    public override void HoldItem(Item item, Player player)
    {
        if (item.HasPrefix(ModContent.PrefixType<PrefixVampiric>())) player.GetModPlayer<StatPlayer>().OnHitLifesteal += PrefixBalance.VAMPIRIC_LIFESTEAL;
    }
}