using System;
using EquipmentEvolved.Assets.Balance;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Magic.Overloaded;

public class OverloadedChargeProjectile : ModProjectile
{
    public int ChargeTicks;
    private bool Fired;
    private bool Initialized;

    public float ShootSpeed;
    public float TotalManaConsumed;
    public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Bullet;

    public ref float ProjToSpawn => ref Projectile.ai[0];
    public ref float ManaCost => ref Projectile.ai[1];

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];

        if (player.dead || !player.active || player.inventory[player.selectedItem].IsAir)
        {
            Projectile.Kill();
            return;
        }

        if (Fired) return;

        // The game already charged the first chunk of mana before this projectile spawned
        if (!Initialized)
        {
            TotalManaConsumed = ManaCost;
            Initialized = true;
        }

        Projectile.Center = player.MountedCenter;
        player.heldProj = Projectile.whoAmI;

        player.ChangeDir(Main.MouseWorld.X < player.Center.X ? -1 : 1);
        player.itemRotation = (Main.MouseWorld - player.Center).ToRotation();
        if (player.direction != 1) player.itemRotation -= MathHelper.Pi;

        player.itemTime = 2;
        player.itemAnimation = 2;

        int drainRateTicks = 10;

        if (ChargeTicks > 0 && ChargeTicks % drainRateTicks == 0)
        {
            if (ManaCost > 0)
            {
                float rampUp = 1f + ChargeTicks / 60f * PrefixBalance.OVERLOADED_MANA_COST_INCREASE_PER_SECOND;
                int manaToDrain = (int)(ManaCost * rampUp);

                if (manaToDrain < 1) manaToDrain = 1;

                if (!player.CheckMana(manaToDrain, true))
                {
                    FireProjectile(player);
                    return;
                }

                TotalManaConsumed += manaToDrain;
            }
        }

        int dustAmount = 1 + ChargeTicks / 60;
        for (int i = 0; i < dustAmount; i++)
        {
            if (Main.rand.NextFloat() < 0.5f)
            {
                float radius = 40 + ChargeTicks / 5f;
                Vector2 dustSpawn = player.Center + Main.rand.NextVector2CircularEdge(radius, radius);
                Vector2 dustVelocity = (player.Center - dustSpawn) * 0.1f;
                Dust.NewDustPerfect(dustSpawn, DustID.MagicMirror, dustVelocity, 100, default, 1f + ChargeTicks / 120f).noGravity = true;
            }
        }

        if (player.channel)
            ChargeTicks++;
        else
            FireProjectile(player);
    }

    private void FireProjectile(Player player)
    {
        Fired = true;

        float damageMult = 1f;

        if (ManaCost > 0)
        {
            float manaMultiplier = TotalManaConsumed / ManaCost;
            damageMult = Math.Max(1f, manaMultiplier * PrefixBalance.OVERLOADED_DAMAGE_EFFICIENCY);
        }
        else
        {
            Item heldItem = player.inventory[player.selectedItem];
            int useTime = heldItem.useTime > 0 ? heldItem.useTime : 20;
            float simulatedMultiples = 1f + (float)ChargeTicks / useTime;
            damageMult = Math.Max(1f, simulatedMultiples * PrefixBalance.OVERLOADED_DAMAGE_EFFICIENCY);
        }

        float scaleMult = MathHelper.Min(PrefixBalance.OVERLOADED_MAX_SCALE_MULT, 1f + (damageMult - 1f) * 0.15f);

        int finalDamage = (int)(Projectile.damage * damageMult);

        Vector2 velocity = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero) * ShootSpeed;

        Projectile newProj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), player.Center, velocity, (int)ProjToSpawn, finalDamage, Projectile.knockBack * damageMult, player.whoAmI);

        newProj.scale *= scaleMult;
        newProj.width = (int)(newProj.width * scaleMult);
        newProj.height = (int)(newProj.height * scaleMult);
        newProj.position.X -= newProj.width / 2f;
        newProj.position.Y -= newProj.height / 2f;

        Projectile.Kill();
    }
}