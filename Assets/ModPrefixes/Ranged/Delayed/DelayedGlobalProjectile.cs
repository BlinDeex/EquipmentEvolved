using System;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Delayed;

public class DelayedGlobalProjectile : GlobalProjectile
{
    public int FramesActive;

    public bool IsDelayedReady;
    public Vector2 LaserEndPoint = Vector2.Zero;
    public Vector2 StoredVelocity = Vector2.Zero;
    public override bool InstancePerEntity => true;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        Item sourceItem = source switch
        {
            EntitySource_ItemUse_WithAmmo ammoSource => ammoSource.Item,
            EntitySource_ItemUse useSource => useSource.Item,
            _ => null
        };

        if (sourceItem == null || !sourceItem.HasPrefix(ModContent.PrefixType<PrefixDelayed>())) return;
        
        if (projectile.aiStyle == ProjAIStyleID.HeldProjectile) return;

        IsDelayedReady = true;
        StoredVelocity = projectile.velocity;
        
        Vector2 startPos = projectile.Center;
        Vector2 direction = StoredVelocity;
        direction.Normalize();

        Vector2 endPos = startPos;
        float maxDistance = 3000f;

        for (float dist = 0; dist < maxDistance; dist += 16f)
        {
            Vector2 checkPos = startPos + direction * dist;
            if (!Collision.CanHitLine(checkPos, 1, 1, checkPos, 1, 1)) break;

            endPos = checkPos;
        }

        LaserEndPoint = endPos;
    }

    public override bool PreAI(Projectile projectile)
    {
        if (!IsDelayedReady) return base.PreAI(projectile);
        
        if (FramesActive == 0)
        {
            FramesActive++;
            return true;
        }
        
        projectile.velocity = Vector2.Zero;
        projectile.timeLeft = 3600;
        projectile.rotation = StoredVelocity.ToRotation() + MathHelper.PiOver2;
        return false;

    }

    public override bool? CanDamage(Projectile projectile)
    {
        return IsDelayedReady ? false : base.CanDamage(projectile);
    }

    public override bool PreDraw(Projectile projectile, ref Color lightColor)
    {
        if (!IsDelayedReady || LaserEndPoint == Vector2.Zero) return base.PreDraw(projectile, ref lightColor);
        
        Texture2D texture = TextureAssets.MagicPixel.Value;
        Vector2 start = projectile.Center;
        Vector2 end = LaserEndPoint;
        float distance = Vector2.Distance(start, end);
        Vector2 direction = end - start;
        direction.Normalize();
        float rotation = direction.ToRotation();
        Vector2 origin = new(0, 0.5f);
        Rectangle rect = new(0, 0, 1, 1);

        float laserWidth = projectile.width > 0 ? projectile.width : 4f;
        
        Main.EntitySpriteDraw(texture, start - Main.screenPosition, rect, Color.Cyan * 0.15f, rotation, origin, new Vector2(distance, laserWidth), SpriteEffects.None);
        
        Main.EntitySpriteDraw(texture, start - Main.screenPosition, rect, Color.White * 0.3f, rotation, origin, new Vector2(distance, Math.Max(1f, laserWidth * 0.2f)), SpriteEffects.None);

        return base.PreDraw(projectile, ref lightColor);
    }
}