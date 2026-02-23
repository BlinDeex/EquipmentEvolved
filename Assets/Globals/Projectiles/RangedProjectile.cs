using System.Collections.Generic;
using System.Linq;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.InstancedGlobalItems;
using EquipmentEvolved.Assets.ModPrefixes.Ranged;
using EquipmentEvolved.Assets.ModPrefixes.Ranged.FlowState;
using EquipmentEvolved.Assets.ModPrefixes.Ranged.Thaumic;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Globals.Projectiles;

public class RangedProjectile : GlobalProjectile
{
    public static void AdaptableSpawn(Projectile projectile, IEntitySource source, InstancedProjectilePrefix projPrefix)
    {
        if (projPrefix.AdaptableSwapped) return;
        int ammoID = projPrefix.AmmoTypeUsed;
        
        if (ammoID < 0) return;
        
        int itemID = projPrefix.ItemUsed.type;

        bool rocketWeapon = AdaptableUtils.ROCKET_WEAPON_IDS.Contains(itemID);
        bool rocketProj = AdaptableUtils.ROCKET_TO_PROJ_IDS.Select(x => x.RocketAmmoID).Contains(ammoID);
        bool rocketInsideNotRocketWeap = rocketProj && !rocketWeapon;

        if (!rocketInsideNotRocketWeap) return;

        int targetSwap = AdaptableUtils.ROCKET_TO_PROJ_IDS.First(x => x.RocketAmmoID == ammoID).RocketProjectileID;
        projectile.Kill();
        Projectile swappedProj = Projectile.NewProjectileDirect(projectile.GetSource_FromThis(), projectile.position, projectile.velocity,
            targetSwap, projectile.damage, projectile.knockBack, projectile.owner);
        swappedProj.GetGlobalProjectile<InstancedProjectilePrefix>().AdaptableSwapped = true;
        
    }

    public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
    {
        InstancedProjectilePrefix projPrefix = projectile.GetGlobalProjectile<InstancedProjectilePrefix>();

        PrefixAscendant(projPrefix, damageDone);
    }

    public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
    {
        InstancedProjectilePrefix projPrefix = projectile.GetGlobalProjectile<InstancedProjectilePrefix>();

        PrefixGiantSlayer(target, ref modifiers, projPrefix);
    }

    private void PrefixGiantSlayer(NPC target, ref NPC.HitModifiers modifiers, InstancedProjectilePrefix projPrefix)
    {
        
        if (projPrefix.GunPrefixType != ModContent.PrefixType<PrefixGiantSlayer>()) return;
        /*
        NPC realTarget = target.realLife != -1 ? Main.npc[target.realLife] : target;
        float targetDamage = realTarget.lifeMax * PrefixBalance.GIANT_SLAYER_PERCENT_DAMAGE;

        WeaponUtils.DealTrueDamage(target, ref modifiers, targetDamage, true);
        */
        WeaponUtils.FullHitCancellation(target, ref modifiers);
    }

    private static void PrefixAscendant(InstancedProjectilePrefix projPrefix, int damageDone)
    {
        if (projPrefix.GunPrefixType != ModContent.PrefixType<PrefixAscendant>()) return;
        if (!projPrefix.ItemUsed.TryGetGlobalItem(out InstancedRangedPrefix rangedPrefix)) return;
        rangedPrefix.DamageDone += damageDone;
    }

    public static void TracerSpawn(Projectile projectile, InstancedProjectilePrefix projPrefix)
    {
        if (projPrefix.GunPrefixType != ModContent.PrefixType<PrefixTracer>()) return;
        projPrefix.Tracer = true;
        projectile.timeLeft = PrefixBalance.TRACER_MAXIMUM_LIFETIME;
    }

    public static void TracerPreAI(Projectile projectile, InstancedProjectilePrefix projPrefix, ref bool runAI)
    {
        if (projPrefix.GunPrefixType != ModContent.PrefixType<PrefixTracer>()) return;
        if (projectile.timeLeft % PrefixBalance.TRACER_PATH_RESOLUTION != 0) return;
        
        projPrefix.TracerPathPoints.Add(projectile.position);
    }

    public static void TracerLineBoom(Projectile projectile, InstancedProjectilePrefix projPrefix)
    {
        int targetDustPoints = projPrefix.TracerPathPoints.Count * PrefixBalance.TRACER_DUST_POSITIONS_BETWEEN_POINTS;
        List<Vector2> dustPositions = new(targetDustPoints);
        List<Vector2> tracerPoints = projPrefix.TracerPathPoints;

        for (int i = 0; i < tracerPoints.Count; i++)
        {
            Vector2 tracerPoint = tracerPoints[i];
            dustPositions.Add(tracerPoint);

            if (i == tracerPoints.Count - 1) break;
            Vector2 nextTracerPoint = tracerPoints[i + 1];
            dustPositions.AddRange(GetEvenlySpacedPoints(tracerPoint, nextTracerPoint, PrefixBalance.TRACER_DUST_POSITIONS_BETWEEN_POINTS));
        }

        foreach (Vector2 dustPosition in dustPositions)
        {
            Dust.NewDust(dustPosition, 0, 0, DustID.ShadowbeamStaff);
        }
    }
    
    public static List<Vector2> GetEvenlySpacedPoints(Vector2 start, Vector2 end, int numPoints)
    {
        List<Vector2> points = [];
        
        Vector2 step = (end - start) / (numPoints - 1);
        
        for (int i = 1; i < numPoints - 1; i++)
        {
            points.Add(start + step * i);
        }

        return points;
    }
    
    public static void ThaumicOnHit(Player player)
    {
        if (ThaumicBuffManager.ValidPositiveBuffs.Count == 0) return;

        // 1. Get all currently active buffs on the player for fast lookups
        HashSet<int> activeBuffs = new HashSet<int>();
        for (int i = 0; i < player.CountBuffs(); i++)
        {
            if (player.buffTime[i] > 0)
            {
                activeBuffs.Add(player.buffType[i]);
            }
        }

        // 2. Separate our valid buffs into "Missing" and "Active"
        List<int> missingBuffs = new();
        List<int> activeValidBuffs = new();

        foreach (int buffID in ThaumicBuffManager.ValidPositiveBuffs)
        {
            if (activeBuffs.Contains(buffID)) activeValidBuffs.Add(buffID);
            else missingBuffs.Add(buffID);
        }

        int buffToApply = -1;
        bool extending = false;

        // 3. Prioritize missing buffs
        if (missingBuffs.Count > 0)
        {
            buffToApply = missingBuffs[Main.rand.Next(missingBuffs.Count)];
        }
        // If we have all possible buffs (very unlikely), extend an active one
        else if (activeValidBuffs.Count > 0)
        {
            buffToApply = activeValidBuffs[Main.rand.Next(activeValidBuffs.Count)];
            extending = true;
        }

        // 4. Apply or Extend
        if (buffToApply != -1)
        {
            if (extending)
            {
                int buffIndex = player.FindBuffIndex(buffToApply);
                if (buffIndex != -1)
                {
                    player.buffTime[buffIndex] += PrefixBalance.THAUMIC_EXTENSION_TICKS;
                    if (player.buffTime[buffIndex] > PrefixBalance.THAUMIC_MAX_DURATION_TICKS)
                    {
                        player.buffTime[buffIndex] = PrefixBalance.THAUMIC_MAX_DURATION_TICKS;
                    }
                }
            }
            else
            {
                player.AddBuff(buffToApply, PrefixBalance.THAUMIC_BASE_DURATION_TICKS);
            }
        }
    }
    
    public static void FlowStateOnHit(NPC target, int weaponDamage)
    {
        var flowNPC = target.GetGlobalNPC<FlowStateNPC>();
        
        flowNPC.FlowStacks++;
        flowNPC.FlowTimer = PrefixBalance.FLOW_STATE_DURATION_TICKS;
        flowNPC.BaseWeaponDamage = weaponDamage;
        flowNPC.DecayTimer = 0;
    }
}