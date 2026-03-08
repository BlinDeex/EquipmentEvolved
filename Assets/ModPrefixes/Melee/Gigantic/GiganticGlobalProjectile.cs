using EquipmentEvolved.Assets.Balance;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.Gigantic;

public class GiganticGlobalProjectile : GlobalProjectile
{
    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source is not IEntitySource_WithStatsFromItem itemSource) return;

        int prefix = itemSource.Item.prefix;

        if (prefix == ModContent.PrefixType<PrefixGigantic>()) ApplyGigantic(projectile);
    }

    private static void ApplyGigantic(Projectile projectile)
    {
        projectile.scale *= PrefixBalance.GIGANTIC_SIZE;

        Vector2 originalCenter = projectile.Center;
        projectile.width = (int)(projectile.width * PrefixBalance.GIGANTIC_SIZE);
        projectile.height = (int)(projectile.height * PrefixBalance.GIGANTIC_SIZE);
        projectile.Center = originalCenter;
    }
}