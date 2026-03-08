using EquipmentEvolved.Assets.Balance;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Magic.Splintering;

public class SplinteringProjectile : ModProjectile
{
    public override string Texture => $"{nameof(EquipmentEvolved)}/Assets/Textures/Projectiles/SplinteringProjectile";

    private ref float initialTargetWhoAmI => ref Projectile.ai[0];

    public override void SetDefaults()
    {
        Projectile.width = 4;
        Projectile.height = 4;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 120;
        Projectile.extraUpdates = 5;
        Projectile.maxPenetrate = 999;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        if (Projectile.timeLeft % 4 == 0) Dust.NewDust(Projectile.position, 1, 1, PrefixBalance.SPLINTERING_DUST_ID);

        Projectile.velocity *= 0.99f;
    }

    public override bool? CanHitNPC(NPC target)
    {
        if ((int)initialTargetWhoAmI == target.whoAmI) return false;

        return !target.friendly;
    }
}