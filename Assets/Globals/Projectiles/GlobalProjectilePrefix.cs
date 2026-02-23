using EquipmentEvolved.Assets.InstancedGlobalItems;
using EquipmentEvolved.Assets.ModPrefixes.Ranged.FlowState;
using EquipmentEvolved.Assets.ModPrefixes.Ranged.Thaumic;
using EquipmentEvolved.Assets.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Globals.Projectiles;

public class GlobalProjectilePrefix : GlobalProjectile
{
    public override bool InstancePerEntity => true;

    // FIX 1: REMOVED 'static'. 
    // This ensures every projectile has its own backup slot. 
    // The static version was getting overwritten when multiple projectiles updated in the same frame.
    private Player _tempRealPlayer;

    public override bool ShouldUpdatePosition(Projectile projectile)
    {
        if (projectile.GetGlobalProjectile<InstancedProjectilePrefix>().TimeStop)
        {
            return false;
        }
        return base.ShouldUpdatePosition(projectile);
    }

    public override bool CanHitPlayer(Projectile projectile, Player target)
    {
        if (projectile.GetGlobalProjectile<InstancedProjectilePrefix>().TimeStop)
        {
            return false;
        }
        return base.CanHitPlayer(projectile, target);
    }

    public override bool? CanHitNPC(Projectile projectile, NPC target)
    {
        if (projectile.GetGlobalProjectile<InstancedProjectilePrefix>().TimeStop)
        {
            return false;
        }
        return base.CanHitNPC(projectile, target);
    }

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        InstancedProjectilePrefix projPrefix = projectile.GetGlobalProjectile<InstancedProjectilePrefix>();
        
        if (PhantomArmorProjectile.CreatingPhantomProjectile != null)
        {
            projPrefix.PhantomParent = PhantomArmorProjectile.CreatingPhantomProjectile;
            if (projPrefix.PhantomParent.ModProjectile is PhantomArmorProjectile { phantom: not null } pap)
            {
                projectile.Center = pap.phantom.Center;
            }
        }

        if (source is IEntitySource_WithStatsFromItem itemUsed)
        {
            projPrefix.GunPrefixType = itemUsed.Item.prefix;
            
            Player player = Main.player[projectile.owner];
            projPrefix.ItemCrit = player.GetWeaponCrit(itemUsed.Item);
            projPrefix.ItemUsed = itemUsed.Item;
        }

        if (source is EntitySource_ItemUse_WithAmmo ammo) projPrefix.AmmoTypeUsed = ammo.AmmoItemIdUsed;

        SummonerProjectile.OnMinionSpawn(projectile, source, projPrefix);
        RangedProjectile.AdaptableSpawn(projectile, source, projPrefix);
        MagicProjectile.OnTripleShotRootSpawn(projectile, projPrefix);
        RangedProjectile.TracerSpawn(projectile, projPrefix);
    }
    
    public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
    {
        InstancedProjectilePrefix projPrefix = projectile.GetGlobalProjectile<InstancedProjectilePrefix>();
        
        if (projPrefix.GunPrefixType == ModContent.PrefixType<PrefixThaumic>())
        {
            Player player = Main.player[projectile.owner];
            RangedProjectile.ThaumicOnHit(player);
        }
        
        if (projPrefix.GunPrefixType == ModContent.PrefixType<PrefixFlowState>())
        {
            RangedProjectile.FlowStateOnHit(target, projectile.damage); 
        }
    
        base.OnHitNPC(projectile, target, hit, damageDone);
    }

    public override bool PreAI(Projectile projectile)
    {
        InstancedProjectilePrefix projPrefix = projectile.GetGlobalProjectile<InstancedProjectilePrefix>();
        
        bool isPhantomSwap = false;

        // Perform Swap
        if (projPrefix.PhantomParent != null && projPrefix.PhantomParent.active && 
            projPrefix.PhantomParent.ModProjectile is PhantomArmorProjectile pap)
        {
            // FIX: ONLY SWAP IF WE ARE THE OWNER
            // Remote clients should not mess with Main.player, or the real player will flicker/vanish.
            if (projectile.owner == Main.myPlayer) 
            {
                if (_tempRealPlayer == null) 
                {
                    _tempRealPlayer = Main.player[projectile.owner];
                    Main.player[projectile.owner] = pap.phantom;
                    isPhantomSwap = true;
                }
            }
        }
        
        bool runPreAI = true;
        
        RangedProjectile.TracerPreAI(projectile, projPrefix, ref runPreAI);
        TimeStop(projPrefix, projectile, ref runPreAI);

        // Restore Player (Only if we swapped)
        if (!runPreAI && isPhantomSwap)
        {
            if (_tempRealPlayer != null)
            {
                Main.player[projectile.owner] = _tempRealPlayer;
                _tempRealPlayer = null;
            }
        }

        return runPreAI;
    }

    public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
    {
        SummonerProjectile.ModifyHitNPCSummoner(projectile, target, ref modifiers);
    }

    public override void PostAI(Projectile projectile)
    {
        InstancedProjectilePrefix projPrefix = projectile.GetGlobalProjectile<InstancedProjectilePrefix>();
        SummonerProjectile.MonarchPostAI(projectile, projPrefix);
        

        // If _tempRealPlayer is null, it means we are either a remote client OR we didn't swap.
        // In either case, we don't need to restore anything.
        if (_tempRealPlayer != null)
        {
            Main.player[projectile.owner] = _tempRealPlayer;
            _tempRealPlayer = null;
        }

        // Position Correction (This is safe to run on all clients)
        // This ensures the bullet looks like it's coming from the Phantom.
        if (projPrefix.PhantomParent != null && projPrefix.PhantomParent.active &&
            projPrefix.PhantomParent.ModProjectile is PhantomArmorProjectile pap)
        {
            // Note: Ensure pap.phantom.MountedCenter is valid on remote clients! 
            // If pap.phantom is not synced, this might need a null check or use projectile.Center.
            if (pap.phantom != null)
            {
                projectile.Center = pap.phantom.Center + (projectile.Center - pap.phantom.MountedCenter);
            }
        }
    }

    private void TimeStop(InstancedProjectilePrefix projPrefix, Projectile projectile, ref bool runAI)
    {
        if (!projPrefix.TimeStop) return;
       
        projectile.timeLeft++; 
        projectile.frameCounter--; 
        
        projPrefix.TimeStopTicks--;

        if (projPrefix.TimeStopTicks > 0)
        {
            runAI = false;
            return;
        }
            
        projPrefix.TimeStop = false;
        projectile.netUpdate = true;
        runAI = false;
    }
    
    public override void OnKill(Projectile projectile, int timeLeft)
    {
        InstancedProjectilePrefix projPrefix = projectile.GetGlobalProjectile<InstancedProjectilePrefix>();
        
        if (projPrefix.Tracer)
        {
            RangedProjectile.TracerLineBoom(projectile, projPrefix);
            return;
        }
    }
}