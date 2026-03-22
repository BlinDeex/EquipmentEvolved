using System;
using EquipmentEvolved.Assets.Balance;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Chestplate.Rebounding;

public class ReboundingModPlayer : ModPlayer
{
    public int ReboundingStacks;
    public int ReboundingTimer;
    public int ShatteredTimer;

    public override void PreUpdate()
    {
        if (ReboundingTimer > 0)
        {
            ReboundingTimer--;
            if (ReboundingTimer <= 0) ReboundingStacks = 0;
        }

        if (ShatteredTimer > 0)
        {
            ShatteredTimer--;
        }
    }

    public override void UpdateEquips()
    {
        bool hasRebounding = false;
        Item reboundingItem = null;

        for (int i = 0; i < 3; i++)
        {
            Item armor = Player.armor[i];
            if (armor != null && !armor.IsAir && armor.prefix == ModContent.PrefixType<PrefixRebounding>())
            {
                hasRebounding = true;
                reboundingItem = armor;
                break;
            }
        }

        if (hasRebounding)
        {
            if (ShatteredTimer > 0)
            {
                Player.statDefense -= reboundingItem.defense;
            }
            else if (ReboundingStacks > 0)
            {
                float totalDefenseBoost = reboundingItem.defense * (PrefixBalance.REBOUNDING_DEFENSE_PER_STACK * ReboundingStacks);
                
                Player.statDefense += (int)MathF.Round(totalDefenseBoost, 2);
            }
        }
        else
        {
            ReboundingStacks = 0;
            ReboundingTimer = 0;
            ShatteredTimer = 0;
        }
    }

    public override void PostHurt(Player.HurtInfo info)
    {
        if (ShatteredTimer > 0) return; 

        bool hasRebounding = false;
        for (int i = 0; i < 3; i++)
        {
            if (Player.armor[i] != null && Player.armor[i].prefix == ModContent.PrefixType<PrefixRebounding>())
            {
                hasRebounding = true;
                break;
            }
        }

        if (!hasRebounding) return;
        
        if (info.Knockback > 4f || info.Damage > Player.statLifeMax2 * 0.15f)
        {
            int shrapnelDamage = (int)(Player.statDefense * PrefixBalance.REBOUNDING_DEFENSE_SCALING);

            for (int i = 0; i < 5; i++)
            {
                Vector2 velocity = new Vector2(Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-8f, -2f));
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, velocity, ProjectileID.CrystalStorm, shrapnelDamage, 2f, Player.whoAmI);
            }
        }

        ReboundingStacks++;
        ReboundingTimer = PrefixBalance.REBOUNDING_STACK_DURATION;
        
        if (ReboundingStacks >= PrefixBalance.REBOUNDING_MAX_STACKS)
        {
            SoundEngine.PlaySound(SoundID.Shatter, Player.Center);
            for (int i = 0; i < 30; i++)
            {
                Dust.NewDust(Player.position, Player.width, Player.height, DustID.Glass);
            }

            CombatText.NewText(Player.getRect(), Color.Red, "SHATTERED!", true);

            ReboundingStacks = 0;
            ReboundingTimer = 0;
            
            // Set shatter duration to 10 seconds (600 ticks). 
            // Optional: You can move this '600' to your PrefixBalance.cs!
            ShatteredTimer = 600; 
        }
    }
}