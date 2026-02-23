using EquipmentEvolved.Assets.Balance;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Projectiles;

public class PhalanxPlatingProjectile : ModProjectile
{
    public override string Texture => $"{nameof(EquipmentEvolved)}/Assets/Textures/Projectiles/SplinteringProjectile";
    
    public override void SetDefaults()
    {
        Projectile.width = 4;
        Projectile.height = 4;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 120;
        Projectile.extraUpdates = 10;
        Projectile.maxPenetrate = 999;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.tileCollide = false;
    }
    
    public override void AI()
    {
        if (Projectile.timeLeft % 4 == 0) Dust.NewDust(Projectile.position, 1, 1, DustID.InfernoFork);
        //Projectile.velocity *= 0.995f;
        //Projectile.velocity.Y += 0.05f;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.Burning, PrefixBalance.PHALANX_BURN_EFFECT_TICKS);
    }
}