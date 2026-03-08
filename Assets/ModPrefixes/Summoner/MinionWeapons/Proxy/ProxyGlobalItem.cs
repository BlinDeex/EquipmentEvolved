using EquipmentEvolved.Assets.Balance;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Summoner.MinionWeapons.Proxy;

public class ProxyGlobalItem : GlobalItem
{
    public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (Main.projPet[type] || ProjectileID.Sets.MinionSacrificable[type] || ProjectileID.Sets.LightPet[type]) return base.Shoot(item, player, source, position, velocity, type, damage, knockback);

        if (player.whoAmI == Main.myPlayer)
        {
            foreach (Projectile p in Main.projectile)
            {
                if (p.active && p.owner == player.whoAmI && p.TryGetGlobalProjectile(out ProxyGlobalProjectile proxy) && proxy.IsProxyMinion)
                {
                    int combinedDamage = (int)((damage + p.damage) * PrefixBalance.PROXY_DAMAGE_EFFICIENCY);
                    
                    Vector2 direction = (Main.MouseWorld - p.Center).SafeNormalize(velocity.SafeNormalize(Vector2.UnitX));
                    Vector2 newVel = direction * velocity.Length();
                    
                    Projectile replicated = Projectile.NewProjectileDirect(source, p.Center, newVel, type, combinedDamage, knockback, player.whoAmI);
                    
                    if (replicated.TryGetGlobalProjectile(out ProxyGlobalProjectile repProxy)) repProxy.IsProxyMinion = false;
                }
            }
        }
        
        return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
    }
}