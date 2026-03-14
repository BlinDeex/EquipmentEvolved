using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Stats.Combat;
using EquipmentEvolved.Assets.Stats.Defense;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Accessory;

public class PrefixStoic : BaseEvolvedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ACCESSORY_REFORGING_MULTIPLIER;

    public LocalizedText DescDefense { get; private set; }
    public LocalizedText DescDamage { get; private set; }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults(); 
        DescDefense = GetLoc(nameof(DescDefense));
        DescDamage = GetLoc(nameof(DescDamage));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "newLine", DescDefense.Format(Math.Round(PrefixBalance.STOIC_DEFENSE_INCREASE * 100, 2)))
        {
            IsModifier = true
        };

        yield return new TooltipLine(Mod, "newLine2", DescDamage.Format(Math.Round(PrefixBalance.STOIC_DAMAGE_DECREASE * 100, 2)))
        {
            IsModifier = true,
            IsModifierBad = true
        };
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        StatPlayer statPlayer = player.GetModPlayer<StatPlayer>();
        statPlayer.AddStat(ModContent.GetInstance<DamageStat>(), PrefixBalance.STOIC_DAMAGE_DECREASE);
        statPlayer.AddStat(ModContent.GetInstance<DefenseMulStat>(), PrefixBalance.STOIC_DEFENSE_INCREASE, StatSource.Accessory);
    }
}