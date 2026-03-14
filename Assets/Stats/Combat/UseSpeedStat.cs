using System;
using EquipmentEvolved.Assets.Core;
using Terraria;

namespace EquipmentEvolved.Assets.Stats.Combat;

public class UseSpeedStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => 
        $"+{Math.Round(totalValue * 100)}% Attack Speed";

    public override float UseSpeedMultiplier(Player player, Item item, float totalValue)
    {
        return 1f + totalValue;
    }
}