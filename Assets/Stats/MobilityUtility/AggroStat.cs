using EquipmentEvolved.Assets.Core;
using Terraria;

namespace EquipmentEvolved.Assets.Stats.MobilityUtility;

public class AggroStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue)
    {
        int aggro = (int)totalValue;
        string key = aggro >= 0 ? "TooltipPositive" : "TooltipNegative";
        return GetLocalization(key).Format(aggro);
    }

    public override void PostUpdateEquips(Player player, float totalValue)
    {
        player.aggro += (int)totalValue;
    }
}