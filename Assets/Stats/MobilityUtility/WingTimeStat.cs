using System;
using EquipmentEvolved.Assets.Core;
using Terraria;

namespace EquipmentEvolved.Assets.Stats.MobilityUtility;

public class WingTimeStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => GetLocalization("Tooltip").Format(Math.Round(totalValue));

    public override void PostUpdateEquips(Player player, float totalValue)
    {
        player.wingTimeMax += (int)Math.Round(totalValue);
    }
}