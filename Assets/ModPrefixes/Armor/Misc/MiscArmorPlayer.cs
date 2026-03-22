using System;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Chestplate.Unyielding;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Leggings.Desperate;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Leggings.Silent;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Leggings.Terra;
using EquipmentEvolved.Assets.Stats.Defense;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Misc;

/// <summary>
///     All armors with no set bonus and/or too simple of a logic to have dedicated class
/// </summary>
public class MiscArmorPlayer : ModPlayer
{
    public bool WaterBreathing { get; set; }

    public override void ResetEffects()
    {
        WaterBreathing = false;
    }

    public override void UpdateEquips()
    {
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
            else if (prefix == ModContent.PrefixType<PrefixUnyielding>())
                HandleUnyielding();
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

    private void HandleUnyielding()
    {
        float missingHealthPct = 1f - (float)Player.statLife / Player.statLifeMax2;
        float dr = missingHealthPct * PrefixBalance.UNYIELDING_MAX_DR;
        
        Player.GetModPlayer<StatPlayer>().AddStat(ModContent.GetInstance<DamageReductionStat>(), dr, StatSource.Chestplate);
    }
}