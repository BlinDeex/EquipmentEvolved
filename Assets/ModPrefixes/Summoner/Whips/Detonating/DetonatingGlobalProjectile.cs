using System;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Summoner.Whips.Detonating;

public class DetonatingGlobalProjectile : GlobalProjectile
{
    public override bool InstancePerEntity => true;

    public bool IsRigged { get; private set; }
    private int _riggedTimer;

    public override void PostAI(Projectile projectile)
    {
        if (!projectile.minion || projectile.owner != Main.myPlayer) return;

        Player player = Main.player[projectile.owner];

        if (player.HeldItem == null || !player.HeldItem.HasPrefix(ModContent.PrefixType<PrefixDetonating>())) 
            return;

        if (!IsRigged)
        {
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.owner == projectile.owner && ProjectileID.Sets.IsAWhip[proj.type])
                {
                    if (proj.Colliding(proj.Hitbox, projectile.Hitbox))
                    {
                        IsRigged = true;
                        _riggedTimer = 0;
                        SoundEngine.PlaySound(SoundID.MenuTick, projectile.Center);
                        break;
                    }
                }
            }
        }
        else
        {
            _riggedTimer++;
            
            if (_riggedTimer >= PrefixBalance.DETONATING_AUTO_EXPLODE_TICKS)
            {
                IsRigged = false;
                Detonate(projectile, projectile.originalDamage);
                return;
            }

            if (Main.rand.NextBool(3))
            {
                Dust spark = Dust.NewDustDirect(projectile.Top - new Vector2(8, 8), 16, 16, DustID.Torch, 0f, -2f, 100, default, 1.5f);
                spark.noGravity = true;
            }
        }
    }

    public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (IsRigged)
        {
            IsRigged = false;
            Detonate(projectile, projectile.originalDamage);
        }
    }

    private void Detonate(Projectile projectile, int baseDamage)
    {
        SoundEngine.PlaySound(SoundID.Item14, projectile.Center);
        Player player = Main.player[projectile.owner];
        
        int explosionDamage = (int)(baseDamage * PrefixBalance.DETONATING_DAMAGE_MULTIPLIER);
        float radius = PrefixBalance.DETONATING_EXPLOSION_RADIUS;
        
        for (int i = 0; i < 30; i++)
        {
            Dust.NewDust(projectile.Center - new Vector2(radius / 2), (int)radius, (int)radius, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
            Dust.NewDust(projectile.Center - new Vector2(radius / 2), (int)radius, (int)radius, DustID.Torch, 0f, 0f, 100, default, 2.5f);
        }
        foreach (NPC npc in Main.ActiveNPCs)
        {
            if (npc.active && !npc.friendly && !npc.dontTakeDamage && npc.lifeMax > 5)
            {
                if (Vector2.Distance(projectile.Center, npc.Center) <= radius)
                {
                    NPC.HitInfo hit = npc.CalculateHitInfo(explosionDamage, Math.Sign(npc.Center.X - projectile.Center.X), false, 0f, DamageClass.Summon);
                    player.StrikeNPCDirect(npc, hit);
                }
            }
        }
    }

    public override void PostDraw(Projectile projectile, Color lightColor)
{
    if (!IsRigged) return;

    Texture2D bombTexture = TextureAssets.Item[ItemID.Bomb].Value;
    Vector2 drawPos = projectile.Center - Main.screenPosition;
    
    float rotation = (float)Math.Sin(_riggedTimer * 0.2f) * 0.3f;
    Vector2 origin = bombTexture.Size() / 2f;
    
    int explodeTicks = PrefixBalance.DETONATING_AUTO_EXPLODE_TICKS;
    int timeLeft = explodeTicks - _riggedTimer;
    float progress = (float)_riggedTimer / explodeTicks;
    
    Color tintColor;
    float scale = 1f;

    if (timeLeft <= 6) 
    {
        tintColor = Color.Red;
        scale = 1.3f;
    }
    else if (timeLeft <= 30) 
    {
        float fadeProgress = 1f - ((timeLeft - 6) / 24f); 
        Color baseFlash = Color.Lerp(lightColor, Color.Red, 0.8f);
        
        tintColor = Color.Lerp(baseFlash, Color.Red, fadeProgress);
        scale = MathHelper.Lerp(1f, 1.3f, fadeProgress);
    }
    else 
    {
        float pulseSpeed = MathHelper.Lerp(0.1f, 1.0f, progress); 
        float pulseAmount = (float)Math.Sin(_riggedTimer * pulseSpeed) * 0.5f + 0.5f; 
        float intensity = Math.Min(progress * 1.2f, 0.8f); 
        
        tintColor = Color.Lerp(lightColor, Color.Red, pulseAmount * intensity);
        
        scale = 1f + (0.15f * pulseAmount * intensity);
    }

    Main.EntitySpriteDraw(
        bombTexture, 
        drawPos, 
        null, 
        tintColor, 
        rotation, 
        origin, 
        scale, 
        SpriteEffects.None, 
        0
    );
}
}