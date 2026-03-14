using System;
using EquipmentEvolved.Assets.Core;

namespace EquipmentEvolved.Assets.Stats.MobilityUtility;

public class WingVerticalAccStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => $"+{Math.Round(totalValue * 100)}% Wing Vertical Acceleration";
}