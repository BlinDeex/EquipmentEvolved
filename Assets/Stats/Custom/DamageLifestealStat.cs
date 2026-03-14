using System;
using EquipmentEvolved.Assets.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Stats.Custom;

public class DamageLifestealStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => 
        $"+{Math.Round(totalValue, 1)}% of Damage Dealt as Lifesteal";

    public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone, float totalValue)
    {
        if (totalValue == 0 || damageDone <= 0) return;
        
        float healingMul = 1f + player.GetModPlayer<StatPlayer>().GetTotalStat(ModContent.GetInstance<HealingMulStat>());
        
        // Value is stored as a raw percentage (e.g. 5 for 5%), so we divide by 100
        float healAmount = damageDone * (totalValue / 100f) * healingMul;
        
        LifeStealStat.ApplyHealOrHurt(player, target, target.Center, healAmount);
    }
}