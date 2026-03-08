using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Magic.Overloaded;

public class OverloadedGlobalItem : GlobalItem
{
    public override bool InstancePerEntity => true;

    public override void HoldItem(Item item, Player player)
    {
        if (item.HasPrefix(ModContent.PrefixType<PrefixOverloaded>())) item.channel = true;
    }

    public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (!item.HasPrefix(ModContent.PrefixType<PrefixOverloaded>())) return base.Shoot(item, player, source, position, velocity, type, damage, knockback);

        int manaCost = player.GetManaCost(item);

        Projectile proj = Projectile.NewProjectileDirect(source, position, Vector2.Zero, ModContent.ProjectileType<OverloadedChargeProjectile>(), damage, knockback, player.whoAmI, type, manaCost);

        if (proj.ModProjectile is not OverloadedChargeProjectile chargeProj) return false;

        float speed = velocity.Length();
        chargeProj.ShootSpeed = speed <= 0 ? 10f : speed;

        return false;
    }
}