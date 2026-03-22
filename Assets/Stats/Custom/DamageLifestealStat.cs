using System;
using EquipmentEvolved.Assets.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Stats.Custom;

public class DamageLifestealStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => GetLocalization("Tooltip").Format(MathF.Round(totalValue, 2));

    public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone, float totalValue)
    {
        if (totalValue == 0 || damageDone <= 0) return;
        
        float healingMul = 1f + player.GetModPlayer<StatPlayer>().GetTotalStat(ModContent.GetInstance<HealingMulStat>());
        
        float rawHealAmount = damageDone * (totalValue / 100f) * healingMul;
        int actualHeal = (int)rawHealAmount;
        
        float leftover = rawHealAmount - actualHeal;
        if (Main.rand.NextFloat() < leftover)
        {
            actualHeal++;
        }
        
        if (actualHeal > 0)
        {
            LifeStealStat.ApplyHealOrHurt(player, target, target.Center, actualHeal);
        }
    }
}