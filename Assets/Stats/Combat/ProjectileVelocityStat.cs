using System;
using EquipmentEvolved.Assets.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Stats.Combat;

public class ProjectileVelocityStat : EquipmentStat
{
    public override StatStackingMode StackingMode => StatStackingMode.Additive;
    public override string FormatTooltip(float totalValue) => GetLocalization("Tooltip").Format(MathF.Round(totalValue * 100), 2);
}

public class VelocityGlobalProjectile : GlobalProjectile
{
    /// <summary>
    /// Safely applies the <see cref="ProjectileVelocityStat"/> to the player's projectiles.
    /// Bypasses Terraria's hardcoded physics limit (which breaks collision and clamps speed if velocity exceeds 16 pixels per frame) 
    /// by mathematically converting excess velocity into <see cref="Projectile.extraUpdates"/>.
    /// </summary>
    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (projectile.owner != Main.myPlayer) return;

        Player owner = Main.player[projectile.owner];
        if (!owner.active) return;

        StatPlayer statPlayer = owner.GetModPlayer<StatPlayer>();
        float velBoost = statPlayer.GetTotalStat(ModContent.GetInstance<ProjectileVelocityStat>());

        if (velBoost <= 0f) return;

        Vector2 targetVelocity = projectile.velocity * (1f + velBoost);
        float targetSpeed = targetVelocity.Length();

        if (targetSpeed > 16f)
        {
            int extraUpdatesToAdd = (int)(targetSpeed / 16f);
            projectile.extraUpdates += extraUpdatesToAdd;
            projectile.velocity = targetVelocity / (extraUpdatesToAdd + 1);
        }
        else
        {
            projectile.velocity = targetVelocity;
        }
    }
}