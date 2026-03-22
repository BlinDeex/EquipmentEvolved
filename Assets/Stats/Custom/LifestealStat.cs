using System;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Stats.Defense;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Stats.Custom;

public class LifeStealStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => GetLocalization("Tooltip").Format(Math.Round(totalValue, 2));

    public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone, float totalValue)
    {
        if (totalValue == 0) return;
        
        float healingMul = 1f + player.GetModPlayer<StatPlayer>().GetTotalStat(ModContent.GetInstance<HealingMulStat>());
        ApplyHealOrHurt(player, target, target.Center, totalValue * healingMul);
    }

    public static void ApplyHealOrHurt(Player player, Entity victim, Vector2 hitPos, float totalHealAmount)
    {
        bool isNegative = totalHealAmount < 0;
        float lifestealAbs = Math.Abs(totalHealAmount);

        int guaranteedHeals = (int)lifestealAbs;
        float leftOverChance = lifestealAbs - guaranteedHeals;

        if (Main.rand.NextFloat() <= leftOverChance) guaranteedHeals++;

        if (guaranteedHeals == 0) return;

        if (isNegative)
        {
            player.Hurt(MiscStuff.GetChaoticWeaponDeath(player.name), guaranteedHeals, 0, armorPenetration: float.MaxValue, dodgeable: false, cooldownCounter: ImmunityCooldownID.General);
            player.immuneTime = 0;
        }
        else
        {
            float healthCapMul = 1f + player.GetModPlayer<StatPlayer>().GetTotalStat(ModContent.GetInstance<HealthCapMulStat>());
            if (healthCapMul < 1f)
            {
                int maxAllowedHealth = (int)(player.statLifeMax2 * healthCapMul);
                int allowedHeal = maxAllowedHealth - player.statLife;

                if (allowedHeal <= 0) return;
                guaranteedHeals = Math.Min(guaranteedHeals, allowedHeal);
            }

            CombatUtils.Lifesteal(victim, hitPos, guaranteedHeals, player);
        }
    }
}