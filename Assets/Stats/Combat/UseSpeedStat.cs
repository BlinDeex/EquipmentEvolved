using System;
using EquipmentEvolved.Assets.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Stats.Combat;

public class UseSpeedStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => GetLocalization("Tooltip").Format(Math.Round(totalValue * 100));

    public override float UseSpeedMultiplier(Player player, Item item, float totalValue)
    {
        return 1f + totalValue;
    }
}

public class UseSpeedModPlayer : ModPlayer
{
    /// <summary>
    /// Bypasses Terraria's hardcoded 60-ticks-per-second attack speed limit.
    /// If the player's attack speed causes the use time to drop below 1 frame, 
    /// this calculates the "overflow" and physically spawns evenly distributed 
    /// sub-frame projectiles along the velocity vector to create a continuous beam.
    /// </summary>
    public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        float vanillaAttackSpeed = Player.GetAttackSpeed(item.DamageType);
        float moddedAttackSpeed = Player.GetModPlayer<StatPlayer>().UseSpeedMultiplier(item);
        float totalAttackSpeed = vanillaAttackSpeed * moddedAttackSpeed;
        float theoreticalUseTime = item.useTime / totalAttackSpeed;

        if (theoreticalUseTime is < 1f and > 0f)
        {
            float totalExpectedShots = 1f / theoreticalUseTime;
            float extraShotsRaw = totalExpectedShots - 1f;
            
            int extraShots = (int)extraShotsRaw;
            float fractionalShotChance = extraShotsRaw - extraShots;
            
            if (Main.rand.NextFloat() < fractionalShotChance)
            {
                extraShots++;
            }

            if (extraShots > 0) 
            {
                int totalProjectilesThisFrame = extraShots + 1;
                Vector2 positionStep = velocity / totalProjectilesThisFrame;

                for (int i = 0; i < extraShots; i++)
                {
                    Vector2 interpolatedSpawnPos = position + (positionStep * (i + 1));
                    Projectile.NewProjectile(source, interpolatedSpawnPos, velocity, type, damage, knockback, Player.whoAmI);
                }
            }
        }

        return base.Shoot(item, source, position, velocity, type, damage, knockback);
    }
}