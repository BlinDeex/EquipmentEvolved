using System;
using EquipmentEvolved.Assets.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Stats.MobilityUtility;

public class WingHorizontalSpeedStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => GetLocalization("Tooltip").Format(Math.Round(totalValue * 100));

    public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration, float totalValue)
    {
        speed *= 1f + totalValue; 
    }
}

public class WingHorizontalSpeedGlobalItem : GlobalItem
{
    public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acc)
    {
        var stat = ModContent.GetInstance<WingHorizontalSpeedStat>();
        float totalValue = player.GetModPlayer<StatPlayer>().GetTotalStat(stat);

        if (totalValue != 0f)
        {
            stat.HorizontalWingSpeeds(player, ref speed, ref acc, totalValue);
        }
    }
}