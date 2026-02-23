using System;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.InstancedGlobalItems;
using EquipmentEvolved.Assets.ModPrefixes.Summoner.MinionWeapons;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Globals.Projectiles;

public class SummonerProjectile : GlobalProjectile
{
    // --- Reincarnation Data ---
    private static float _lastMonarchStacks;
    private static int _lastMonarchOwner = -1;
    private static uint _lastMonarchDeathTime;

    public static void OnMinionSpawn(Projectile projectile, IEntitySource source, InstancedProjectilePrefix projPrefix)
    {
        // A. Logic for the Minion itself (The Staff spawning the minion)
        if (projectile.minion)
        {
            Item heldItem = Main.LocalPlayer.HeldItem;
            if (heldItem.shoot == projectile.type) 
            {
                HandleFrenziedSpawn(projectile, source, projPrefix);
                HandleMonarchSpawn(projectile, source, projPrefix);
            }
        }
        
        // B. Logic for Child Projectiles (Minions shooting lasers/orbs)
        // THIS IS WHERE WE WRITE TO MonarchChild/MonarchDamageMult
        HandleMonarchChildSpawn(projectile, source, projPrefix);
    }
    public static void ModifyHitNPCSummoner(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
    {
        if (!projectile.TryGetGlobalProjectile(out InstancedProjectilePrefix prefix)) return;
        
        // If this is a child shot (Stardust Cell Shot, etc.), multiply the final damage
        if (prefix.MonarchChild && prefix.MonarchDamageMult > 1f)
        {
            modifiers.FinalDamage *= prefix.MonarchDamageMult;
        }
    }
    
    public static void MonarchPostAI(Projectile projectile, InstancedProjectilePrefix projPrefix)
    {
        if (projPrefix.Monarch)
        {
            (float damageMult, float scale, float slots) stats = GetMonarchStats(projPrefix.MonarchStacks);
            
            // FIX: Desert Tiger (and potentially others) resets scale every frame in AI.
            // We force it back here. 
            // ID 831 = StormTigerGem (The Minion)
            if (projectile.type == ProjectileID.StormTigerGem || Math.Abs(projectile.scale - stats.scale) > 0.01f)
            {
                projectile.scale = stats.scale;
                
                // For the Tiger, we might also want to ensure the hitbox is resized if it feels too small.
                // But usually Scale is enough for visuals/contact.
                // If you notice the Tiger clipping inside enemies without hitting, uncomment this:
                // projectile.Resize((int)(32 * stats.scale), (int)(32 * stats.scale)); 
            }
        }
    }

    private static void HandleMonarchChildSpawn(Projectile projectile, IEntitySource source, InstancedProjectilePrefix projPrefix)
    {
        // We check if the "Parent" of this new projectile is another Projectile (The Minion)
        if (source is EntitySource_Parent parentSource && parentSource.Entity is Projectile parentProj)
        {
            // We check if that Parent Minion is a Monarch
            if (parentProj.TryGetGlobalProjectile(out InstancedProjectilePrefix parentPrefix) && parentPrefix.Monarch)
            {
                // Retrieve the Monarch's current stats
                (float damageMult, float scale, float slots) stats = GetMonarchStats(parentPrefix.MonarchStacks);

                // --- WRITE HAPPENS HERE ---
                projPrefix.MonarchChild = true;
                projPrefix.MonarchDamageMult = stats.damageMult;

                // Apply Scale immediately (Visuals)
                projectile.scale *= stats.scale;
                
                // Debug to confirm it's working (Delete later)
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    // Main.NewText($"Monarch Child Spawned! Dmg: {stats.damageMult}x", Color.Gold);
                }

                if (Main.netMode != NetmodeID.Server)
                {
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.GoldFlame);
                }
            }
        }
    }

    // ... (Keep existing OnKill, HandleFrenziedSpawn, HandleMonarchSpawn, UpdateMonarchStats, GetMonarchStats) ...
    // ... (Paste the rest of the previous file here) ...
    
    public override void OnKill(Projectile projectile, int timeLeft)
    {
        if (projectile.TryGetGlobalProjectile(out InstancedProjectilePrefix prefix) && prefix.Monarch)
        {
            _lastMonarchStacks = prefix.MonarchStacks;
            _lastMonarchOwner = projectile.owner;
            _lastMonarchDeathTime = Main.GameUpdateCount;
        }
        base.OnKill(projectile, timeLeft);
    }

    private static void HandleFrenziedSpawn(Projectile projectile, IEntitySource source, InstancedProjectilePrefix projPrefix)
    {
        if (source is IEntitySource_WithStatsFromItem itemSource && itemSource.Item.prefix == ModContent.PrefixType<PrefixFrenzied>())
        {
            projPrefix.Frenzied = true;
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                projectile.netUpdate = true;
            }
        }
    }

    private static void HandleMonarchSpawn(Projectile projectile, IEntitySource source, InstancedProjectilePrefix projPrefix)
    {
        if (source is IEntitySource_WithStatsFromItem itemSource && itemSource.Item.prefix == ModContent.PrefixType<PrefixMonarch>())
        {
            Player player = Main.player[projectile.owner];
            projPrefix.Monarch = true;

            Projectile existingMonarch = null;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == player.whoAmI && p.type == projectile.type && p.whoAmI != projectile.whoAmI)
                {
                    if (p.GetGlobalProjectile<InstancedProjectilePrefix>().Monarch)
                    {
                        existingMonarch = p;
                        break;
                    }
                }
            }

            if (existingMonarch != null)
            {
                InstancedProjectilePrefix existingPrefix = existingMonarch.GetGlobalProjectile<InstancedProjectilePrefix>();
                float projectedSlots = existingMonarch.minionSlots + projectile.minionSlots;
                
                if (projectedSlots <= player.maxMinions)
                {
                    existingPrefix.MonarchStacks += 1f;
                    existingMonarch.minionSlots += projectile.minionSlots;
                    UpdateMonarchStats(existingMonarch, existingPrefix);
                }
                
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int k = 0; k < 15; k++)
                    {
                        Dust dust = Dust.NewDustDirect(existingMonarch.position, existingMonarch.width, existingMonarch.height, DustID.GoldFlame);
                        dust.velocity *= 1.5f;
                        dust.noGravity = true;
                    }
                }
                
                projectile.active = false;
                projectile.Kill();
            }
            else
            {
                bool isReincarnation = _lastMonarchOwner == projectile.owner && 
                                       (Main.GameUpdateCount - _lastMonarchDeathTime <= 1);

                if (isReincarnation)
                {
                    float potentialNewStacks = _lastMonarchStacks + 1f;
                    if (potentialNewStacks > player.maxMinions)
                        projPrefix.MonarchStacks = _lastMonarchStacks;
                    else
                        projPrefix.MonarchStacks = potentialNewStacks;
                }
                else
                {
                    projPrefix.MonarchStacks = 1f;
                }

                UpdateMonarchStats(projectile, projPrefix);
            }
        }
    }

    public static (float damageMult, float scale, float slots) GetMonarchStats(float stacks)
    {
        float effectiveStacks = stacks;
        if (stacks > PrefixBalance.MONARCH_SOFT_CAP)
        {
            float excess = stacks - PrefixBalance.MONARCH_SOFT_CAP;
            effectiveStacks = PrefixBalance.MONARCH_SOFT_CAP + (excess * PrefixBalance.MONARCH_DIMINISHING_RETURN);
        }

        float growth;
        if (stacks <= PrefixBalance.MONARCH_SOFT_CAP)
            growth = (stacks - 1) * PrefixBalance.MONARCH_SIZE_PER_STACK;
        else
        {
            float baseGrowth = (PrefixBalance.MONARCH_SOFT_CAP - 1) * PrefixBalance.MONARCH_SIZE_PER_STACK;
            float excessGrowth = (stacks - PrefixBalance.MONARCH_SOFT_CAP) * (PrefixBalance.MONARCH_SIZE_PER_STACK * PrefixBalance.MONARCH_DIMINISHING_RETURN);
            growth = baseGrowth + excessGrowth;
        }

        float finalScale = PrefixBalance.MONARCH_BASE_SIZE + growth;
        float efficiencyMultiplier = 1f + ((effectiveStacks - 1) * PrefixBalance.MONARCH_EFFICIENCY_BONUS_PER_STACK);
        float finalDamageMult = effectiveStacks * efficiencyMultiplier;

        float finalSlots = stacks;
        if (Main.LocalPlayer.active && finalSlots > Main.LocalPlayer.maxMinions) 
            finalSlots = Main.LocalPlayer.maxMinions;

        return (finalDamageMult, finalScale, finalSlots);
    }

    public static void UpdateMonarchStats(Projectile projectile, InstancedProjectilePrefix prefix)
    {
        (float damageMult, float scale, float slots) stats = GetMonarchStats(prefix.MonarchStacks);
        
        projectile.scale = stats.scale;
        
        if (prefix.BaseDamage == 0) prefix.BaseDamage = projectile.originalDamage;
        projectile.originalDamage = (int)(prefix.BaseDamage * stats.damageMult);

        // Clamping minion slots
        Player player = Main.player[projectile.owner];
        float slotsIdeally = stats.slots; 
        if (slotsIdeally > player.maxMinions) slotsIdeally = player.maxMinions;
        projectile.minionSlots = slotsIdeally; 

        if (Main.netMode != NetmodeID.SinglePlayer)
        {
            projectile.netUpdate = true;
        }
    }
}