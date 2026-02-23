using System;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.InstancedGlobalItems;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Melee;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Globals.Projectiles;

public class MeleeProjectile : GlobalProjectile
{
    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        // Check if the projectile was spawned by a weapon
        if (source is IEntitySource_WithStatsFromItem itemSource)
        {
            int prefix = itemSource.Item.prefix;

            if (prefix == ModContent.PrefixType<PrefixGigantic>())
            {
                ApplyGigantic(projectile);
            }
            else if (prefix == ModContent.PrefixType<PrefixUltraLight>())
            {
                ApplyUltraLight(projectile, itemSource.Item);
            }
        }
    }

    private static void ApplyGigantic(Projectile projectile)
    {
        projectile.scale *= PrefixBalance.GIGANTIC_SIZE;

        Vector2 originalCenter = projectile.Center;
        projectile.width = (int)(projectile.width * PrefixBalance.GIGANTIC_SIZE);
        projectile.height = (int)(projectile.height * PrefixBalance.GIGANTIC_SIZE);
        projectile.Center = originalCenter;
    }

    private static void ApplyUltraLight(Projectile projectile, Item item)
    {
        // 1. If it already uses Static Immunity, just scale it down
        if (projectile.usesIDStaticNPCImmunity)
        {
            projectile.idStaticNPCHitCooldown = (int)(projectile.idStaticNPCHitCooldown * PrefixBalance.ULTRA_LIGHT_USE);
            if (projectile.idStaticNPCHitCooldown < 1) projectile.idStaticNPCHitCooldown = 1;
        }
        else
        {
            // 2. Force Local Immunity to bypass the 10-tick Global Immunity cap
            projectile.usesLocalNPCImmunity = true;

            // If it already had a local cooldown (like Yoyos), scale it
            if (projectile.localNPCHitCooldown > 0)
            {
                projectile.localNPCHitCooldown = (int)(projectile.localNPCHitCooldown * PrefixBalance.ULTRA_LIGHT_USE);
            }
            else
            {
                // Otherwise (like Chainsaws), set it to the item's blazing fast useTime
                projectile.localNPCHitCooldown = Math.Max(1, item.useTime);
            }
        }
    }

    public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
    {
        InstancedProjectilePrefix projPrefix = projectile.GetGlobalProjectile<InstancedProjectilePrefix>();

        if (projPrefix.GunPrefixType == ModContent.PrefixType<PrefixPerceptive>())
        {
            PrefixPerceptive(target, ref modifiers, projPrefix);
        }
    }

    public static void PrefixPerceptive(NPC target, ref NPC.HitModifiers modifiers,
        InstancedProjectilePrefix projPrefix)
    {
        modifiers.ModifyHitInfo += (ref NPC.HitInfo info) =>
        {
            if (!info.Crit) return;
            
            int currentCrit = projPrefix.ItemCrit;
            if (currentCrit <= 100) return;

            // --- Calc Logic ---
            float damageMult = 1f;
            int critToCheck = currentCrit - 100;
            int tierReached = 0;
            const int MAX_TIER = 6;

            while (critToCheck > 0 && tierReached < MAX_TIER)
            {
                if (Main.rand.Next(0, 100) < critToCheck)
                {
                    damageMult += (tierReached == 0) ? 1f : 0.5f;
                    tierReached++;
                }
                else break;
                critToCheck -= 100;
            }
            // ------------------

            if (tierReached <= 0) return;
            
            info.Damage = (int)(info.Damage * damageMult);
            info.HideCombatText = true;

            if (Main.netMode == NetmodeID.Server) return;
            // 1. Show Local
            WeaponUtils.SpawnPerceptiveText(target.Center, info.Damage, tierReached);
                    
            // 2. Send Packet to others
            if (Main.netMode != NetmodeID.MultiplayerClient) return;
            ModPacket packet = ModContent.GetInstance<EquipmentEvolved>().GetPacket();
            packet.Write((byte)MessageType.PerceptiveCritEffect);
            packet.Write(target.Center.X);
            packet.Write(target.Center.Y);
            packet.Write(info.Damage);
            packet.Write((byte)tierReached);
            packet.Send();
        };
    }
}