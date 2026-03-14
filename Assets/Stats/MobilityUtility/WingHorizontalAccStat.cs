using System;
using EquipmentEvolved.Assets.Core;

namespace EquipmentEvolved.Assets.Stats.MobilityUtility;

public class WingHorizontalAccStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => $"+{Math.Round(totalValue * 100)}% Wing Horizontal Acceleration";
}