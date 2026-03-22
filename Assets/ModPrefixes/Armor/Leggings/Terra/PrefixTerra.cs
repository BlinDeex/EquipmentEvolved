using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc; // New namespace
using EquipmentEvolved.Assets.ModPrefixes.Core;
using EquipmentEvolved.Assets.Stats.Defense;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Leggings.Terra;

public class PrefixTerra : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Leggings;

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        int defensePercent = (int)Math.Round(PrefixBalance.TERRA_DEFENSE_MULT * 100);

        yield return new TooltipLine(Mod, "TerraDesc", Description.Format(defensePercent))
        {
            IsModifier = true,
            IsModifierBad = false
        };
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        if (player.velocity.Y != 0) return;
        StatPlayer statPlayer = player.GetModPlayer<StatPlayer>();
        
        Item leggings = player.armor[2];
        if (leggings == null || leggings.IsAir) return;
        
        int leggingsDefense = leggings.defense;
        
        int defenseBonus = (int)Math.Round(leggingsDefense * PrefixBalance.TERRA_DEFENSE_MULT);
        statPlayer.AddStat(ModContent.GetInstance<FlatDefenseStat>(), defenseBonus, StatSource.Leggings);
    }
}