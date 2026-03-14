using EquipmentEvolved.Assets.Core;

namespace EquipmentEvolved.Assets.Stats.MobilityUtility;

public class CoinDropOnHitStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => 
        $"+{(int)totalValue} Coins Dropped on Hit";
}