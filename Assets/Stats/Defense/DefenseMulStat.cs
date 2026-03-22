using System;
using EquipmentEvolved.Assets.Core;
using Terraria;

namespace EquipmentEvolved.Assets.Stats.Defense;

public class DefenseMulStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue)
    {
        int percent = (int)Math.Round(totalValue * 100);
        string key = percent >= 0 ? "TooltipPositive" : "TooltipNegative";
        return GetLocalization(key).Format(percent);
    }

    public override void UpdateEquips(Player player, float totalValue)
    {
        player.statDefense.FinalMultiplier *= 1f + totalValue;
    }
}