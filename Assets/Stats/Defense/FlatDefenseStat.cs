using System;
using EquipmentEvolved.Assets.Core;
using Terraria;

namespace EquipmentEvolved.Assets.Stats.Defense;

public class FlatDefenseStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => GetLocalization("Tooltip").Format(Math.Round(totalValue, 2));

    public override void UpdateEquips(Player player, float totalValue)
    {
        player.statDefense += (int)totalValue;
    }
}