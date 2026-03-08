using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.GiantSlayer;

public class GiantSlayerGlobalItem : GlobalItem
{
    public override void HoldItem(Item item, Player player)
    {
        if (item.HasPrefix(ModContent.PrefixType<PrefixGiantSlayer>())) player.GetModPlayer<StatPlayer>().TrueDamagePercentage += PrefixBalance.EQUALIZER_PERCENT_DAMAGE;
    }
}