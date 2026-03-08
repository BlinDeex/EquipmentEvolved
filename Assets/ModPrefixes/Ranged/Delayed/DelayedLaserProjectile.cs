using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Delayed;

public class DelayedLaserProjectile : ModProjectile
{
    public override string Texture => "Terraria/Images/Item_0";
    
    public Vector2 EndPoint => new(Projectile.ai[0], Projectile.ai[1]);

    public float LaserWidth => Projectile.ai[2] > 0 ? Projectile.ai[2] : 4f;

    public override void SetDefaults()
    {
        Projectile.width = 10;
        Projectile.height = 10;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 15;
        Projectile.tileCollide = false; 
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1; 
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float collisionPoint = 0f;
        
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, EndPoint, LaserWidth, ref collisionPoint);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.MagicPixel.Value;

        Vector2 start = Projectile.Center;
        Vector2 end = EndPoint;
        float distance = Vector2.Distance(start, end);
        Vector2 direction = end - start;
        direction.Normalize();
        float rotation = direction.ToRotation();
        Vector2 origin = new(0, 0.5f);
        Rectangle rect = new(0, 0, 1, 1);

        float opacity = Projectile.timeLeft / 15f;
        Color baseColor = Color.Cyan;
        
        Main.EntitySpriteDraw(texture, start - Main.screenPosition, rect, baseColor * (0.4f * opacity), rotation, origin, new Vector2(distance, LaserWidth * 1.5f), SpriteEffects.None);

        Main.EntitySpriteDraw(texture, start - Main.screenPosition, rect, baseColor * (0.9f * opacity), rotation, origin, new Vector2(distance, LaserWidth * 0.7f), SpriteEffects.None);

        Main.EntitySpriteDraw(texture, start - Main.screenPosition, rect, Color.White * opacity, rotation, origin, new Vector2(distance, Math.Max(1f, LaserWidth * 0.3f)), SpriteEffects.None);

        return false;
    }
}