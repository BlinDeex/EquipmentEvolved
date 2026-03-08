using System;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Chestplate.Rebounding;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Chestplate.Unyielding;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Chestplate.Void;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Leggings.Desperate;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Leggings.Silent;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Leggings.Terra;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Misc;

/// <summary>
///     All armors with no set bonus and/or too simple of a logic to have dedicated class
/// </summary>
public class MiscArmorPlayer : ModPlayer
{
    public int ReboundingStacks;
    public int ReboundingTimer;
    public bool WaterBreathing { get; set; }

    public override void ResetEffects()
    {
        WaterBreathing = false;
    }

    public override void PreUpdate()
    {
        if (ReboundingTimer > 0)
        {
            ReboundingTimer--;
            if (ReboundingTimer <= 0) ReboundingStacks = 0;
        }
    }

    public override void UpdateEquips()
    {
        bool hasRebounding = false;

        for (int i = 0; i < 3; i++)
        {
            Item armor = Player.armor[i];
            if (armor == null || armor.IsAir) continue;

            int prefix = armor.prefix;

            if (prefix == ModContent.PrefixType<PrefixSilent>())
                HandleSilent();
            else if (prefix == ModContent.PrefixType<PrefixTerra>())
                HandleTerra(armor);
            else if (prefix == ModContent.PrefixType<PrefixDesperate>())
                HandleDesperate();
            else if (prefix == ModContent.PrefixType<PrefixVoid>())
                HandleVoid();
            else if (prefix == ModContent.PrefixType<PrefixUnyielding>())
                HandleUnyielding();
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
        float missingHealthPct = 1f - (float)Player.statLife / Player.statLifeMax2;
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
        float missingHealthPct = 1f - (float)Player.statLife / Player.statLifeMax2;
        float dr = missingHealthPct * PrefixBalance.UNYIELDING_MAX_DR;

        Player.GetModPlayer<StatPlayer>().DamageReductionMul -= dr;
    }

    private void HandleReboundingEquip(bool hasRebounding)
    {
        if (hasRebounding && ReboundingStacks > 0)
            Player.statDefense += ReboundingStacks * PrefixBalance.REBOUNDING_DEFENSE_PER_STACK;
        else if (!hasRebounding)
        {
            ReboundingStacks = 0;
            ReboundingTimer = 0;
        }
    }

    private void HandleReboundingHurt(Player.HurtInfo info, int slotIndex)
    {
        if (info.Knockback > 4f || info.Damage > Player.statLifeMax2 * 0.15f)
        {
            int shrapnelDamage = Player.statDefense * PrefixBalance.REBOUNDING_DEFENSE_SCALING;

            for (int i = 0; i < 5; i++)
            {
                Vector2 velocity = new(Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-8f, -2f));
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, velocity, ProjectileID.CrystalStorm, shrapnelDamage, 2f, Player.whoAmI);
            }
        }

        ReboundingStacks++;
        ReboundingTimer = PrefixBalance.REBOUNDING_STACK_DURATION;

        if (ReboundingStacks >= PrefixBalance.REBOUNDING_MAX_STACKS)
        {
            Player.armor[slotIndex] = new Item();

            if (slotIndex == 0)
                Player.head = -1;
            else if (slotIndex == 1)
                Player.body = -1;
            else if (slotIndex == 2) Player.legs = -1;

            if (Main.myPlayer == Player.whoAmI) Recipe.FindRecipes();

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
}