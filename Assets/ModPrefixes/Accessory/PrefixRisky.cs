using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.Stats.Combat;
using EquipmentEvolved.Assets.Stats.Defense;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Accessory;

public class PrefixRisky : BaseEvolvedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ACCESSORY_REFORGING_MULTIPLIER;

    public LocalizedText DescDefense { get; private set; }
    public LocalizedText DamageDesc { get; private set; }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults(); // Gets the standard setup safely
        DescDefense = GetLoc(nameof(DescDefense));
        DamageDesc = LocalizationManager.GetSharedLocalizedText(LocalizationManager.XDamageAdded);
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "newLine", DescDefense.Format(PrefixBalance.RISKY_DEFENSE_DECREASE * 100))
        {
            IsModifier = true,
            IsModifierBad = true
        };

        yield return new TooltipLine(Mod, "newLine2", DamageDesc.Format(Math.Round(PrefixBalance.RISKY_DAMAGE_INCREASE * 100, 2)))
        {
            IsModifier = true
        };
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        StatPlayer statPlayer = player.GetModPlayer<StatPlayer>();
        statPlayer.AddStat(ModContent.GetInstance<DamageStat>(), PrefixBalance.RISKY_DAMAGE_INCREASE, StatSource.Accessory);
        statPlayer.AddStat(ModContent.GetInstance<DefenseMulStat>(), PrefixBalance.RISKY_DEFENSE_DECREASE, StatSource.Accessory);
    }
}