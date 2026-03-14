using System;
using EquipmentEvolved.Assets.Core;

namespace EquipmentEvolved.Assets.Stats.MobilityUtility;

public class CoinDropMulStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => 
        $"+{Math.Round(totalValue * 100)}% Coin Drops";
}