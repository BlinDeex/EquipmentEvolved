using System.Diagnostics.CodeAnalysis;
using EquipmentEvolved.Assets.Balance;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.Payback;

public class PaybackCoinProjectile : ModProjectile
{
    public override string Texture => "Terraria/Images/Item_" + ItemID.GoldCoin;

    public override void SetDefaults()
    {
        Projectile.width = 14;
        Projectile.height = 14;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 300;
        Projectile.tileCollide = false;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void AI()
    {
        Projectile.ai[0]++;

        Lighting.AddLight(Projectile.Center, 0.4f, 0.3f, 0.1f);

        Projectile.frameCounter++;
        if (Projectile.frameCounter >= 6)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;
            if (Projectile.frame >= 8) Projectile.frame = 0;
        }

        if (Projectile.ai[0] < 45)
        {
            Projectile.velocity.Y += 0.2f;
            Projectile.velocity.X *= 0.98f;
        }
        else
        {
            NPC target = Main.npc[(int)Projectile.ai[1]];

            if (target.active && !target.dontTakeDamage)
            {
                Vector2 direction = target.Center - Projectile.Center;
                float distance = direction.Length();
                direction.Normalize();

                float speed = 24f;

                if (distance < 50f)
                    Projectile.velocity = direction * speed;
                else
                    Projectile.velocity = (Projectile.velocity * 20f + direction * speed) / 21f;
            }
            else
                Projectile.velocity.Y += 0.2f;
        }
    }

    public override bool? CanHitNPC(NPC target)
    {
        return Projectile.ai[0] < 45 ? false : base.CanHitNPC(target);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        int coinItemID = (int)Projectile.ai[2];
        int coinTextureIndex = 0;

        switch (coinItemID)
        {
            case ItemID.CopperCoin: coinTextureIndex = 0; break;
            case ItemID.SilverCoin: coinTextureIndex = 1; break;
            case ItemID.GoldCoin: coinTextureIndex = 2; break;
            case ItemID.PlatinumCoin: coinTextureIndex = 3; break;
            default: return false;
        }

        Texture2D texture = TextureAssets.Coin[coinTextureIndex].Value;

        int frameHeight = texture.Height / 8;
        Rectangle sourceRect = new(0, Projectile.frame * frameHeight, texture.Width, frameHeight);
        Vector2 origin = sourceRect.Size() / 2f;

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, sourceRect, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);

        return false;
    }

    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public override void OnKill(int timeLeft)
    {
        if (Main.rand.NextFloat() <= PrefixBalance.PAYBACK_COIN_RETURN_CHANCE)
        {
            int coinItemID = (int)Projectile.ai[2];

            int itemIdx = Item.NewItem(Projectile.GetSource_DropAsItem(), Projectile.Hitbox, coinItemID
            );

            if (Main.item.IndexInRange(itemIdx)) Main.item[itemIdx].velocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-3f, -1f));
        }
        
        int dustType = DustID.SilverCoin;
        if (Projectile.ai[2] == ItemID.CopperCoin) dustType = DustID.CopperCoin;

        if (Projectile.ai[2] == ItemID.GoldCoin) dustType = DustID.GoldCoin;

        if (Projectile.ai[2] == ItemID.PlatinumCoin) dustType = DustID.PlatinumCoin;

        for (int i = 0; i < 7; i++)
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, Projectile.velocity.X * -0.5f, Projectile.velocity.Y * -0.5f);
        }
    }
}