using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Summoner.MinionWeapons.Proxy;

public class ProxyGlobalProjectile : GlobalProjectile
{
    private static readonly HashSet<int> InvertedMinions = //TODO maybe somehow decide whether minion is inverted by inspecting its ai programatically
    [
        ProjectileID.Raven,
        ProjectileID.Retanimini,
        ProjectileID.Spazmamini
    ];

    private bool _initializedPos;
    private Vector2 _spawnPos;

    public bool IsProxyMinion;
    public int ProxyDuration;
    public override bool InstancePerEntity => true;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source is EntitySource_ItemUse itemSource && itemSource.Item.HasPrefix(ModContent.PrefixType<PrefixProxy>()))
        {
            if (projectile.minion || projectile.sentry || ProjectileID.Sets.MinionSacrificable[projectile.type])
            {
                IsProxyMinion = true;
                ProxyDuration = PrefixBalance.PROXY_MAX_DURATION_TICKS;
                projectile.timeLeft = ProxyDuration + 2;
            }
        }
    }

    public override bool PreAI(Projectile projectile)
    {
        if (IsProxyMinion)
        {
            if (!_initializedPos)
            {
                _spawnPos = projectile.Center + Main.rand.NextVector2CircularEdge(120f, 120f);
                _initializedPos = true;
            }

            projectile.Center = _spawnPos;
            projectile.velocity = Vector2.Zero;

            ProxyDuration--;
            if (ProxyDuration <= 0)
            {
                for (int i = 0; i < 25; i++)
                {
                    Vector2 outwardVelocity = Main.rand.NextVector2Circular(4f, 4f);
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Smoke, outwardVelocity.X, outwardVelocity.Y, 100, default, 1.8f);
                }

                projectile.Kill();
                return false;
            }

            if (projectile.owner == Main.myPlayer)
            {
                Vector2 toMouse = Main.MouseWorld - projectile.Center;
                
                projectile.spriteDirection = toMouse.X > 0 ? 1 : -1;
                projectile.rotation = toMouse.ToRotation();
                
                if (InvertedMinions.Contains(projectile.type))
                {
                    projectile.spriteDirection *= -1;
                    projectile.rotation += MathHelper.Pi;
                }
                
                if (projectile.spriteDirection == -1) projectile.rotation += MathHelper.Pi;
            }
            
            float lifePercentage = (float)ProxyDuration / PrefixBalance.PROXY_MAX_DURATION_TICKS;
            
            int smokeChance = (int)MathHelper.Lerp(2f, 25f, lifePercentage);
            smokeChance = Math.Max(1, smokeChance);

            if (Main.rand.NextBool(smokeChance))
            {
                float smokeScale = MathHelper.Lerp(1.5f, 1f, lifePercentage);
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Smoke, 0f, -2f, 100, default, smokeScale);
            }

            if (++projectile.frameCounter >= 6)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type]) projectile.frame = 0;
            }

            return false;
        }

        return base.PreAI(projectile);
    }

    public override void PostDraw(Projectile projectile, Color lightColor)
    {
        if (!IsProxyMinion || ProxyDuration <= 0) return;
        
        float progress = (float)ProxyDuration / PrefixBalance.PROXY_MAX_DURATION_TICKS;
        Vector2 barPos = projectile.Bottom + new Vector2(0, 8) - Main.screenPosition;

        int barWidth = 32;
        int barHeight = 4;

        Rectangle bgRect = new((int)barPos.X - barWidth / 2, (int)barPos.Y, barWidth, barHeight);
        Rectangle fgRect = new((int)barPos.X - barWidth / 2, (int)barPos.Y, (int)(barWidth * progress), barHeight);

        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, bgRect, Color.Black * 0.7f);
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, fgRect, Color.Cyan * 0.9f);
    }
}