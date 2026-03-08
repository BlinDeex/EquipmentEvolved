using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Magic.TripleShot;

public class TripleShotGlobalProjectile : GlobalProjectile
{
    public bool IsTripleShotClone;

    public override bool InstancePerEntity => true;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (IsTripleShotClone) return;

        if (source is IEntitySource_WithStatsFromItem itemSource && itemSource.Item.HasPrefix(ModContent.PrefixType<PrefixTripleShot>()))
        {
            (Vector2 vel1, Vector2 vel2) = CombatUtils.TripleShotRotatedVelocities(projectile.velocity, PrefixBalance.TRIPLE_SHOT_DEGREES, PrefixBalance.TRIPLE_SHOT_DEGREES_VARIATION);

            Projectile newProj1 = Projectile.NewProjectileDirect(projectile.GetSource_FromThis(), projectile.position, vel1, projectile.type, projectile.damage, projectile.knockBack, projectile.owner,
                projectile.ai[0], projectile.ai[1], projectile.ai[2]);

            Projectile newProj2 = Projectile.NewProjectileDirect(projectile.GetSource_FromThis(), projectile.position, vel2, projectile.type, projectile.damage, projectile.knockBack, projectile.owner,
                projectile.ai[0], projectile.ai[1], projectile.ai[2]);

            newProj1.GetGlobalProjectile<TripleShotGlobalProjectile>().IsTripleShotClone = true;
            newProj2.GetGlobalProjectile<TripleShotGlobalProjectile>().IsTripleShotClone = true;
        }
    }
}