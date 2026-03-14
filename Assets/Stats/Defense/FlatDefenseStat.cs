using EquipmentEvolved.Assets.Core;
using Terraria;

namespace EquipmentEvolved.Assets.Stats.Defense;

public class FlatDefenseStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => 
        $"+{(int)totalValue} Defense";

    public override void PostUpdateEquips(Player player, float totalValue)
    {
        player.statDefense += (int)totalValue;
    }
}