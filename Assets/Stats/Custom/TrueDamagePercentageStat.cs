using System;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Stats.Custom;

public class TrueDamagePercentageStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => 
        $"+{Math.Round(totalValue * 100)}% Target Max Health as True Damage";

    public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone, float totalValue)
    {
        if (totalValue <= 0) return;

        float mul = 1f + player.GetModPlayer<StatPlayer>().GetTotalStat(ModContent.GetInstance<TrueDamageMulStat>());
        NPC realTarget = target.realLife != -1 ? Main.npc[target.realLife] : target;
        
        float targetDamage = realTarget.lifeMax * (totalValue * mul);
        WeaponUtils.InflictTrueDamage(realTarget, targetDamage);
    }
}