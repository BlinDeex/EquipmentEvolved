using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.InstancedGlobalItems;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPlayers;
using EquipmentEvolved.Assets.ModPrefixes.Magic;
using EquipmentEvolved.Assets.Projectiles;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Globals.Projectiles;

public class MagicProjectile : GlobalProjectile
{
    public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
    {
        if (!projectile.TryGetGlobalProjectile(out InstancedProjectilePrefix projPrefix))
        {
            UtilMethods.LogMessage($"Couldn't get {nameof(InstancedProjectilePrefix)}!", LogType.Error);
            return;
        }
        
        PrefixSplintering(projectile, target, ref modifiers);
    }

    private static void PrefixSplintering(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
    {
        if (Main.player[projectile.owner].HeldItem.prefix != ModContent.PrefixType<PrefixSplintering>()) return;
        if (projectile.type == ModContent.ProjectileType<SplinteringProjectile>()) return;
        if (Main.rand.NextFloat() > PrefixBalance.SPLINTERING_CHANCE && !PrefixBalance.DEV_MODE) return;

        int nodes = Main.rand.Next(PrefixBalance.SPLINTERING_MIN_SHARD_NODES,
            PrefixBalance.SPLINTERING_MAX_SHARD_NODES);

        modifiers.ModifyHitInfo += (ref NPC.HitInfo info) =>
        {
            if (info.Crit) nodes *= PrefixBalance.SPLINTERING_SHARD_NODES_CRIT_MULT;
        };

        for (int i = 0; i < nodes; i++)
        {
            Vector2 nodePos = UtilMethods.GetRandomPositionInRectangle(target.Hitbox, Main.rand);
            int projectiles = Main.rand.Next(PrefixBalance.SPLINTERING_MIN_NODE_PROJ,
                PrefixBalance.SPLINTERING_MAX_NODE_PROJ);
            float variation = Main.rand.NextFloat(-PrefixBalance.SPLINTERING_PROJ_VELOCITY_VARIATION_MUL,
                PrefixBalance.SPLINTERING_PROJ_VELOCITY_VARIATION_MUL);
            float velMult = PrefixBalance.SPLINTERING_PROJ_VELOCITY + variation;

            for (int j = 0; j < projectiles; j++)
            {
                Vector2 projPos = UtilMethods.RandomPointInCircle(nodePos.X, nodePos.Y, 1f, Main.rand);
                Vector2 dir = Vector2.Zero;
                dir.X = Main.rand.NextFloat();
                dir.Y = 1 - dir.X;
                dir.X = Main.rand.NextBool() ? dir.X : -dir.X;
                dir.Y = Main.rand.NextBool() ? dir.Y : -dir.Y;
                Vector2 velocity = dir * velMult;

                Projectile.NewProjectile(new EntitySource_Misc("SplinteringSpawn"), projPos, velocity,
                    ModContent.ProjectileType<SplinteringProjectile>(), projectile.damage, 0, Main.LocalPlayer.whoAmI, ai0: target.whoAmI);
            }
        }
    }

    public static void OnTripleShotRootSpawn(Projectile projectile, InstancedProjectilePrefix projPrefix)
    {
        if (projPrefix.GunPrefixType != ModContent.PrefixType<PrefixTripleShot>()) return;
        if (projPrefix.TripleShootClone) return;


        (Vector2, Vector2) newVelocities = CombatUtils.TripleShotRotatedVelocities(projectile.velocity,
            PrefixBalance.TRIPLE_SHOT_DEGREES, PrefixBalance.TRIPLE_SHOT_DEGREES_VARIATION);

        Projectile newProj1 = Projectile.NewProjectileDirect(projectile.GetSource_FromThis(), projectile.position,
            newVelocities.Item1, projectile.type, projectile.damage, projectile.knockBack, projectile.owner,
            projectile.ai[0], projectile.ai[1], projectile.ai[2]);

        Projectile newProj2 = Projectile.NewProjectileDirect(projectile.GetSource_FromThis(), projectile.position,
            newVelocities.Item2, projectile.type, projectile.damage, projectile.knockBack, projectile.owner,
            projectile.ai[0], projectile.ai[1], projectile.ai[2]);

        newProj1.GetGlobalProjectile<InstancedProjectilePrefix>().TripleShootClone = true;
        newProj2.GetGlobalProjectile<InstancedProjectilePrefix>().TripleShootClone = true;
    }
}