using EquipmentEvolved.Assets.Balance;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Leggings.Quantum;

public class QuantumAfterimageProjectile : ModProjectile
{
    private int WaitTimer => (int)Projectile.ai[0];
    private Player Owner => Main.player[Projectile.owner];

    public override string Texture => "Terraria/Images/Projectile_0";

    public override void SetDefaults()
    {
        Projectile.width = 30;
        Projectile.height = 40;
        Projectile.friendly = true; 
        Projectile.hostile = false;
        Projectile.tileCollide = false; 
        Projectile.penetrate = -1; 
        Projectile.timeLeft = 300;
        Projectile.DamageType = DamageClass.Melee; 
    }

    public override void AI()
    {
        if (!Owner.active || Owner.dead)
        {
            Projectile.Kill();
            return;
        }

        Projectile.ai[0]++;

        if (WaitTimer < PrefixBalance.QUANTUM_AFTERIMAGE_WAIT_TICKS)
        {
            Projectile.velocity = Vector2.Zero;
        }
        else
        {
            if (WaitTimer == PrefixBalance.QUANTUM_AFTERIMAGE_WAIT_TICKS)
            {
                // TODO: Play custom afterimage dashing/whoosh sound effect here!
                // Terraria.Audio.SoundEngine.PlaySound(SoundID.Item8 with { Pitch = 0.5f }, Projectile.Center);
            }

            Vector2 targetCenter = Owner.Center;
        
            Projectile.velocity = Projectile.Center.DirectionTo(targetCenter) * PrefixBalance.QUANTUM_AFTERIMAGE_RETURN_SPEED;
            
            int dustDensity = 30;

            for (int i = 0; i < dustDensity; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.MushroomTorch, 0f, 0f, 0, default, 0.7f);
            }

            if (Projectile.Distance(targetCenter) < PrefixBalance.QUANTUM_AFTERIMAGE_MERGE_DISTANCE)
            {
                // Optional TODO: Add a small "merge" sound effect or visual puff here!
                Projectile.Kill();
            }
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Vector2 originalPosition = Owner.position;
        int originalImmuneAlpha = Owner.immuneAlpha;
        int originalDirection = Owner.direction;
        
        Owner.position = Projectile.position; 
        Owner.immuneAlpha = 150; 
        
        int travelDirection = System.Math.Sign(Owner.Center.X - Projectile.Center.X);
        if (travelDirection != 0) 
        {
            Owner.direction = travelDirection;
        }
        
        Main.PlayerRenderer.DrawPlayer(Main.Camera, Owner, Owner.position, 0f, Owner.fullRotationOrigin, 0f, 1f);
        Owner.position = originalPosition;
        Owner.immuneAlpha = originalImmuneAlpha;
        Owner.direction = originalDirection;

        return false;
    }
}