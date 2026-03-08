using System.Linq;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Adaptable;

public class AdaptableGlobalProjectile : GlobalProjectile
{
    public bool AdaptableSwapped;
    
    public override bool InstancePerEntity => true;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (AdaptableSwapped) return;
        
        if (source is EntitySource_ItemUse_WithAmmo ammoSource)
        {
            Item weapon = ammoSource.Item;
            int ammoID = ammoSource.AmmoItemIdUsed;

            if (ammoID < 0) return;
            
            if (!weapon.HasPrefix(ModContent.PrefixType<PrefixAdaptable>())) return;

            int itemID = weapon.type;
            
            bool rocketWeapon = AdaptableUtils.ROCKET_WEAPON_IDS.Contains(itemID);
            bool rocketProj = AdaptableUtils.ROCKET_TO_PROJ_IDS.Select(x => x.RocketAmmoID).Contains(ammoID);
            bool rocketInsideNotRocketWeap = rocketProj && !rocketWeapon;

            if (!rocketInsideNotRocketWeap) return;

            int targetSwap = AdaptableUtils.ROCKET_TO_PROJ_IDS.First(x => x.RocketAmmoID == ammoID).RocketProjectileID;

            projectile.Kill();

            Projectile swappedProj = Projectile.NewProjectileDirect(projectile.GetSource_FromThis(), projectile.position, projectile.velocity, targetSwap, projectile.damage, projectile.knockBack,
                projectile.owner);
            
            swappedProj.GetGlobalProjectile<AdaptableGlobalProjectile>().AdaptableSwapped = true;
        }
    }
}