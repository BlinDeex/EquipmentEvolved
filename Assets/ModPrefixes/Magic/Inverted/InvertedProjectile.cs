using EquipmentEvolved.Assets.Balance;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Magic.Inverted;

public class InvertedProjectile : ModProjectile
{
    private float velMult = 1;

    public override string Texture => $"{nameof(EquipmentEvolved)}/Assets/Textures/Projectiles/SplinteringProjectile";

    public override void SetDefaults()
    {
        Projectile.width = 4;
        Projectile.height = 4;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 80;
        Projectile.extraUpdates = 5;
        Projectile.maxPenetrate = 999;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        if (Projectile.timeLeft == 79) velMult += Main.rand.NextFloat(-0.03f, 0.03f);

        if (Projectile.timeLeft % 4 == 0) Dust.NewDust(Projectile.position, 1, 1, PrefixBalance.INVERTED_MANA_SURGE_DUST_ID);

        Projectile.velocity.X *= velMult;
        Projectile.velocity = new Vector2(Projectile.velocity.X, Projectile.velocity.Y + 0.1f * velMult);
    }
}