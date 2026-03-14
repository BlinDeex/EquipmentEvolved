using System;
using EquipmentEvolved.Assets.Core;
using Terraria;

namespace EquipmentEvolved.Assets.Stats.Defense;

public class DefenseMulStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue)
    {
        int percent = (int)Math.Round(totalValue * 100);
        return percent < 0 ? $"{percent}% Defense" : $"+{percent}% Defense";
    }

    public override void PostUpdateEquips(Player player, float totalValue)
    {
        player.statDefense.FinalMultiplier *= (1f + totalValue);
    }
}