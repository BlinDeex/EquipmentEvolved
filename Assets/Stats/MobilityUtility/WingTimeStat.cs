using EquipmentEvolved.Assets.Core;
using Terraria;

namespace EquipmentEvolved.Assets.Stats.MobilityUtility;

public class WingTimeStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => 
        $"+{(int)totalValue} Wing Time";

    public override void PostUpdateEquips(Player player, float totalValue)
    {
        player.wingTimeMax += (int)totalValue;
    }
}