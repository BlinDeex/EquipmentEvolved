using System;
using EquipmentEvolved.Assets.Core;
using Terraria;

namespace EquipmentEvolved.Assets.Stats.MobilityUtility;

public class PickSpeedStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue)
    {
        int percent = (int)Math.Round(totalValue * 100);
        string key = percent >= 0 ? "TooltipPositive" : "TooltipNegative";
        return GetLocalization(key).Format(percent);
    }

    public override void PostUpdateEquips(Player player, float totalValue)
    {
        // Base speed is 1f. A totalValue of 0.15f makes it 1.15f.
        float speedMul = 1f + totalValue;
        if (speedMul <= 0f) speedMul = 0.01f;

        player.pickSpeed /= speedMul;
    }
}