using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.Payback;

public class PaybackPrefixPlayer : ModPlayer
{
    public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (hit.DamageType.CountsAsClass(DamageClass.Melee)) TryPayback(target, hit, damageDone);
    }

    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (proj.type == ModContent.ProjectileType<PaybackCoinProjectile>()) return;

        if (hit.DamageType.CountsAsClass(DamageClass.Melee)) TryPayback(target, hit, damageDone);
    }

    private void TryPayback(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Item heldItem = Player.inventory[Player.selectedItem];

        if (heldItem != null && !heldItem.IsAir && heldItem.HasPrefix(ModContent.PrefixType<PrefixPayback>()))
        {
            int coinsToThrow = Main.rand.Next(PrefixBalance.PAYBACK_MIN_COINS, PrefixBalance.PAYBACK_MAX_COINS + 1);
            int coinsThrown = 0;

            for (int i = 53; i >= 0; i--)
            {
                Item invItem = Player.inventory[i];

                if (invItem.type >= ItemID.CopperCoin && invItem.type <= ItemID.PlatinumCoin && invItem.stack > 0)
                {
                    int consumedType = invItem.type;

                    invItem.stack--;
                    if (invItem.stack <= 0) invItem.TurnToAir();

                    float coinDamageMult = consumedType switch
                    {
                        ItemID.CopperCoin => PrefixBalance.PAYBACK_COPPER_MULT,
                        ItemID.SilverCoin => PrefixBalance.PAYBACK_SILVER_MULT,
                        ItemID.GoldCoin => PrefixBalance.PAYBACK_GOLD_MULT,
                        ItemID.PlatinumCoin => PrefixBalance.PAYBACK_PLATINUM_MULT,
                        _ => 1.0f
                    };

                    int projDamage = (int)(damageDone * coinDamageMult);
                    Vector2 fountVelocity = new(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-7f, -3f));

                    Projectile.NewProjectile(Player.GetSource_ItemUse(heldItem), Player.Center, fountVelocity, ModContent.ProjectileType<PaybackCoinProjectile>(), projDamage, hit.Knockback * 0.5f,
                        Player.whoAmI, 0f, target.whoAmI, consumedType);

                    coinsThrown++;
                    if (coinsThrown >= coinsToThrow) break;
                }
            }
        }
    }
}