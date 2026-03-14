using System;
using EquipmentEvolved.Assets.Core;
using Terraria;

namespace EquipmentEvolved.Assets.Stats.Defense;

public class HealthCapMulStat : EquipmentStat
{
    public override StatStackingMode StackingMode => StatStackingMode.Multiplicative;

    public override string FormatTooltip(float totalValue)
    {
        int percent = (int)Math.Round(totalValue * 100);
        return percent < 0 ? $"{percent}% Maximum Health Cap" : $"+{percent}% Maximum Health Cap";
    }

    public override void UpdateLifeRegen(Player player, float totalValue)
    {
        float capMul = 1f + totalValue;
        if (capMul < 1f)
        {
            int maxAllowedHealth = (int)(player.statLifeMax2 * capMul);
            if (player.statLife >= maxAllowedHealth)
            {
                player.statLife = maxAllowedHealth;
                player.lifeRegen = 0; 
                player.lifeRegenTime = 0; 
            }
        }
    }
    
    public override bool CanUseItem(Player player, Item item, float totalValue)
    {
        // Only intercept items that heal life
        if (item.healLife > 0)
        {
            float capMul = 1f + totalValue;
            if (capMul < 1f)
            {
                int cappedHealth = (int)(player.statLifeMax2 * capMul);
                if (player.statLife >= cappedHealth)
                {
                    return false; // Block the potion/healing item
                }
            }
        }

        return true;
    }

    public override void GetHealLife(Player player, Item item, bool quickHeal, ref int healValue, float totalValue)
    {
        float capMul = 1f + totalValue;
        if (capMul < 1f)
        {
            int maxAllowedHealth = (int)(player.statLifeMax2 * capMul);
            int allowedHeal = maxAllowedHealth - player.statLife;

            if (allowedHeal <= 0)
            {
                healValue = 0;
                return;
            }

            healValue = Math.Min(healValue, allowedHeal);
        }
    }
}