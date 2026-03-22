using System;
using EquipmentEvolved.Assets.Core;
using Terraria;

namespace EquipmentEvolved.Assets.Stats.Custom;

public class HealingMulStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => GetLocalization("Tooltip").Format(Math.Round(totalValue * 100));

    public override void GetHealLife(Player player, Item item, bool quickHeal, ref int healValue, float totalValue)
    {
        // Total value of 0.15 makes this a 1.15x multiplier
        healValue = (int)(healValue * (1f + totalValue));
    }
}