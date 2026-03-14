using System;
using EquipmentEvolved.Assets.Core;

namespace EquipmentEvolved.Assets.Stats.MobilityUtility;

public class MoveSpeedStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => 
        $"+{Math.Round(totalValue * 100)}% Movement Speed";
}