using System;
using EquipmentEvolved.Assets.Core;
using Terraria;

namespace EquipmentEvolved.Assets.Stats.Defense;

public class RegenStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => GetLocalization("Tooltip").Format(Math.Round(totalValue, 1));

    public override void UpdateLifeRegen(Player player, float totalValue)
    {
        int baseRegen = (int)totalValue;
        float leftover = Math.Abs(totalValue - baseRegen);

        if (Main.rand.NextFloat() < leftover)
        {
            baseRegen += Math.Sign(totalValue); 
        }

        player.lifeRegen += baseRegen;
    }
}