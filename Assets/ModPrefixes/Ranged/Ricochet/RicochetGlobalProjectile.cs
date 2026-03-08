using System.Diagnostics.CodeAnalysis;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Ricochet;

public class RicochetGlobalProjectile : GlobalProjectile
{
    public float CurrentDamageMultiplier = 1f;
    public float CurrentScaleMultiplier = 1f;

    public bool IsRicochet;
    
    public bool IsRicochetChain;
    public int SplitsRemaining;
    public override bool InstancePerEntity => true;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source is IEntitySource_WithStatsFromItem itemSource && itemSource.Item.HasPrefix(ModContent.PrefixType<PrefixRicochet>()))
        {
            IsRicochet = true;
            IsRicochetChain = true;
            SplitsRemaining = PrefixBalance.RICOCHET_MAX_SPLITS;
            CurrentDamageMultiplier = 1f;
            CurrentScaleMultiplier = 1f;
        }
        
        if (source is EntitySource_Parent parentSource && parentSource.Entity is Projectile parentProj)
        {
            if (!parentProj.TryGetGlobalProjectile(out RicochetGlobalProjectile parentRicochet) || !parentRicochet.IsRicochetChain) return;
            
            IsRicochetChain = true;
            CurrentDamageMultiplier = parentRicochet.CurrentDamageMultiplier;
            CurrentScaleMultiplier = parentRicochet.CurrentScaleMultiplier;
            projectile.damage = (int)(projectile.damage * CurrentDamageMultiplier);
            projectile.scale *= CurrentScaleMultiplier;
        }
    }

    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
    {
        if (!IsRicochet || SplitsRemaining <= 0) return base.OnTileCollide(projectile, oldVelocity);

        SplitsRemaining--;
        
        Vector2 bounceVelocity = projectile.velocity;
        if (projectile.velocity.X != oldVelocity.X) bounceVelocity.X = -oldVelocity.X;

        if (projectile.velocity.Y != oldVelocity.Y) bounceVelocity.Y = -oldVelocity.Y;

        int numSplits = PrefixBalance.RICOCHET_PROJECTILES_PER_SPLIT;
        float spread = MathHelper.ToRadians(PrefixBalance.RICOCHET_SPREAD_DEGREES);

        for (int i = 0; i < numSplits; i++)
        {
            float rotationOffset = numSplits > 1 ? MathHelper.Lerp(-spread / 2f, spread / 2f, i / (float)(numSplits - 1)) : 0f;

            Vector2 splitVelocity = bounceVelocity.RotatedBy(rotationOffset);
            Vector2 spawnPos = projectile.position - oldVelocity * 0.5f;
            
            int newDamage = (int)(projectile.damage * PrefixBalance.RICOCHET_DAMAGE_MULTIPLIER_PER_SPLIT);

            Projectile newProj = Projectile.NewProjectileDirect(projectile.GetSource_FromThis(), spawnPos, splitVelocity, projectile.type, newDamage, projectile.knockBack, projectile.owner,
                projectile.ai[0], projectile.ai[1], projectile.ai[2]);
            
            newProj.scale = projectile.scale * PrefixBalance.RICOCHET_SCALE_MULTIPLIER_PER_SPLIT;

            if (!newProj.TryGetGlobalProjectile(out RicochetGlobalProjectile ricochetProj)) continue;
            
            ricochetProj.IsRicochet = true;
            ricochetProj.IsRicochetChain = true;
            ricochetProj.SplitsRemaining = SplitsRemaining;
            
            ricochetProj.CurrentDamageMultiplier = CurrentDamageMultiplier * PrefixBalance.RICOCHET_DAMAGE_MULTIPLIER_PER_SPLIT;
            ricochetProj.CurrentScaleMultiplier = CurrentScaleMultiplier * PrefixBalance.RICOCHET_SCALE_MULTIPLIER_PER_SPLIT;
        }
        
        return true;
    }
}