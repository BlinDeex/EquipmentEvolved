using System;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Leggings;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Chestplate; // Make sure this matches your folder structure
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPlayers.Armor.Universal;

/// <summary>
/// All armors with no set bonus and/or too simple of a logic to have dedicated class
/// </summary>
public class MiscArmorPlayer : ModPlayer
{
    public bool WaterBreathing { get; set; }

    // Trackers for Rebounding
    public int ReboundingStacks;
    public int ReboundingTimer;

    public override void ResetEffects()
    {
        WaterBreathing = false;
    }

    public override void PreUpdate()
    {
        // Handle Rebounding stack decay
        if (ReboundingTimer > 0)
        {
            ReboundingTimer--;
            if (ReboundingTimer <= 0)
            {
                ReboundingStacks = 0;
            }
        }
    }

    public override void UpdateEquips()
    {
        bool hasRebounding = false;

        // Loop through all 3 armor slots. This keeps the code clean and 
        // ensures it works even if you move these prefixes to different slots later.
        for (int i = 0; i < 3; i++)
        {
            Item armor = Player.armor[i];
            if (armor == null || armor.IsAir) continue;

            int prefix = armor.prefix;

            if (prefix == ModContent.PrefixType<PrefixSilent>()) HandleSilent();
            else if (prefix == ModContent.PrefixType<PrefixTerra>()) HandleTerra(armor);
            else if (prefix == ModContent.PrefixType<PrefixDesperate>()) HandleDesperate();
            else if (prefix == ModContent.PrefixType<PrefixVoid>()) HandleVoid();
            else if (prefix == ModContent.PrefixType<PrefixUnyielding>()) HandleUnyielding();
            else if (prefix == ModContent.PrefixType<PrefixRebounding>()) hasRebounding = true;
        }

        HandleReboundingEquip(hasRebounding);
    }

    public override void PostHurt(Player.HurtInfo info)
    {
        for (int i = 0; i < 3; i++)
        {
            if (Player.armor[i] != null && Player.armor[i].prefix == ModContent.PrefixType<PrefixRebounding>())
            {
                HandleReboundingHurt(info, i);
                break;
            }
        }
    }

    // =================================================================================
    // PREFIX EXTRACTION METHODS
    // =================================================================================

    private void HandleSilent()
    {
        Player.aggro -= PrefixBalance.SILENT_AGGRO_REDUCTION;
    }

    private void HandleTerra(Item item)
    {
        if (Player.velocity.Y == 0)
        {
            int defenseBonus = (int)Math.Round(item.defense * PrefixBalance.TERRA_DEFENSE_MULT);
            Player.statDefense += defenseBonus;
        }
    }

    private void HandleDesperate()
    {
        float missingHealthPct = 1f - ((float)Player.statLife / Player.statLifeMax2);
        float speedBonus = missingHealthPct * PrefixBalance.DESPERATE_MAX_SPEED_BONUS;
        
        Player.moveSpeed += speedBonus;
        Player.maxRunSpeed += Player.maxRunSpeed * speedBonus;
    }

    private void HandleVoid()
    {
        Player.GetModPlayer<StatPlayer>().TrueDamageMul += PrefixBalance.VOID_TRUE_DAMAGE_BONUS;
    }

    private void HandleUnyielding()
    {
        float missingHealthPct = 1f - ((float)Player.statLife / Player.statLifeMax2);
        float dr = missingHealthPct * PrefixBalance.UNYIELDING_MAX_DR;
        
        Player.GetModPlayer<StatPlayer>().DamageReductionMul -= dr;
    }

    private void HandleReboundingEquip(bool hasRebounding)
    {
        if (hasRebounding && ReboundingStacks > 0)
        {
            Player.statDefense += ReboundingStacks * PrefixBalance.REBOUNDING_DEFENSE_PER_STACK;
        }
        else if (!hasRebounding)
        {
            // Failsafe: clears stacks if the player unequips the item mid-fight
            ReboundingStacks = 0;
            ReboundingTimer = 0;
        }
    }

    private void HandleReboundingHurt(Player.HurtInfo info, int slotIndex)
    {
        // Trigger on big knockback hits OR hits that deal > 15% of max health
        if (info.Knockback > 4f || info.Damage > Player.statLifeMax2 * 0.15f)
        {
            int shrapnelDamage = (int)(Player.statDefense * PrefixBalance.REBOUNDING_DEFENSE_SCALING);

            for (int i = 0; i < 5; i++)
            {
                Vector2 velocity = new Vector2(Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-8f, -2f));
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, velocity,
                    ProjectileID.CrystalStorm, shrapnelDamage, 2f, Player.whoAmI);
            }
        }

        ReboundingStacks++;
        ReboundingTimer = PrefixBalance.REBOUNDING_STACK_DURATION;

        // THE SHATTER MECHANIC
        if (ReboundingStacks >= PrefixBalance.REBOUNDING_MAX_STACKS)
        {
            // 1. Completely overwrite the item reference with a blank one
            Player.armor[slotIndex] = new Item(); 
            
            // 2. Manually clear the player's visual equip cache for the current frame
            if (slotIndex == 0) Player.head = -1;
            else if (slotIndex == 1) Player.body = -1;
            else if (slotIndex == 2) Player.legs = -1;

            // 3. Force Terraria's UI and recipe list to immediately recognize the lost item
            if (Main.myPlayer == Player.whoAmI)
            {
                Recipe.FindRecipes(false);
            }

            // Visuals & Sound
            SoundEngine.PlaySound(SoundID.Shatter, Player.Center);
            for (int i = 0; i < 30; i++)
            {
                Dust.NewDust(Player.position, Player.width, Player.height, DustID.Glass);
            }

            CombatText.NewText(Player.getRect(), Color.Red, "SHATTERED!", true);

            ReboundingStacks = 0;
            ReboundingTimer = 0;
        }
    }

    // =================================================================================
    // MISC METHODS
    // =================================================================================

    public int WaterBreathingPrefix()
    {
        int breath = Player.breath;
        
        if (!WaterBreathing) return breath;
        if (Player.breath >= Player.breathMax) return breath;
        if (Player.breathCD != Player.breathCDMax - 1) return breath;
        
        float dice = Main.rand.NextFloat();
        if (dice > 0.67f) return breath;
        
        breath++;
        
        return breath;
    }
}