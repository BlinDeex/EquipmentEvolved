using System;
using EquipmentEvolved.Assets.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Stats.Defense;

public class MaxHealthMulStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => GetLocalization("Tooltip").Format(Math.Round(totalValue * 100));

    public override void ModifyMaxStats(Player player, ref StatModifier health, ref StatModifier mana, float totalValue)
    {
        // totalValue of 0.15 adds a 15% multiplier
        health *= (1f + totalValue);
    }
}