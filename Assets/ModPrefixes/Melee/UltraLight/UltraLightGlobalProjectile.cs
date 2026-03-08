using System;
using EquipmentEvolved.Assets.Balance;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.UltraLight;

public class UltraLightGlobalProjectile : GlobalProjectile
{
    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source is not IEntitySource_WithStatsFromItem itemSource) return;

        int prefix = itemSource.Item.prefix;
        if (prefix == ModContent.PrefixType<PrefixUltraLight>()) ApplyUltraLight(projectile, itemSource.Item);
    }

    private static void ApplyUltraLight(Projectile projectile, Item item)
    {
        if (projectile.usesIDStaticNPCImmunity)
        {
            projectile.idStaticNPCHitCooldown = (int)(projectile.idStaticNPCHitCooldown * PrefixBalance.ULTRA_LIGHT_USE);
            if (projectile.idStaticNPCHitCooldown < 1) projectile.idStaticNPCHitCooldown = 1;
        }
        else
        {
            projectile.usesLocalNPCImmunity = true;

            if (projectile.localNPCHitCooldown > 0)
                projectile.localNPCHitCooldown = (int)(projectile.localNPCHitCooldown * PrefixBalance.ULTRA_LIGHT_USE);
            else
                projectile.localNPCHitCooldown = Math.Max(1, item.useTime);
        }
    }
}