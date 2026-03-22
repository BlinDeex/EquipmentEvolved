using System;
using EquipmentEvolved.Assets.Core;
using Terraria;

namespace EquipmentEvolved.Assets.Stats.MobilityUtility;

public class MoveSpeedStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => GetLocalization("Tooltip").Format(Math.Round(totalValue * 100));
    
    public override void PostUpdateEquips(Player player, float totalValue)
    {
        if (totalValue == 0) return;
        
        player.moveSpeed += totalValue;
        player.maxRunSpeed *= 1f + totalValue;
    }
}