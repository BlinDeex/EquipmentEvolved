using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Stats.Custom;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.GiantSlayer;

public class GiantSlayerGlobalItem : GlobalItem
{
    public override void HoldItem(Item item, Player player)
    {
        if (item.HasPrefix(ModContent.PrefixType<PrefixGiantSlayer>()))
        {
            player.GetModPlayer<StatPlayer>().AddStat(ModContent.GetInstance<TrueDamagePercentageStat>(), PrefixBalance.EQUALIZER_PERCENT_DAMAGE, StatSource.Weapon);
        }
    }
}