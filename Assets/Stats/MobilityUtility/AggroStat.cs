using EquipmentEvolved.Assets.Core;
using Terraria;

namespace EquipmentEvolved.Assets.Stats.MobilityUtility;

public class AggroStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue)
    {
        return totalValue >= 0 ? $"+{(int)totalValue} Enemy Aggro" : $"{(int)totalValue} Enemy Aggro";
    }

    public override void PostUpdateEquips(Player player, float totalValue)
    {
        player.aggro += (int)totalValue;
    }
}