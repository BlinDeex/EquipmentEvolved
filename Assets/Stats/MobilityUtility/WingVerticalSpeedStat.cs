using System;
using EquipmentEvolved.Assets.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Stats.MobilityUtility;

public class WingVerticalSpeedStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => GetLocalization("Tooltip").Format(Math.Round(totalValue * 100));

    public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend, float totalValue)
    {
        maxAscentMultiplier *= 1f + totalValue;
        maxCanAscendMultiplier *= 1f + totalValue; 
    }
}

public class WingVerticalSpeedGlobalItem : GlobalItem
{
    public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
    {
        var stat = ModContent.GetInstance<WingVerticalSpeedStat>();
        float totalValue = player.GetModPlayer<StatPlayer>().GetTotalStat(stat);

        if (totalValue != 0f)
        {
            stat.VerticalWingSpeeds(player, ref ascentWhenFalling, ref ascentWhenRising, ref maxCanAscendMultiplier, ref maxAscentMultiplier, ref constantAscend, totalValue);
        }
    }
}