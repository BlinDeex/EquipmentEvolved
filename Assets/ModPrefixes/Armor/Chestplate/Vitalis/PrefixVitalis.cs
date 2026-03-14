using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Core;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using EquipmentEvolved.Assets.Stats.Custom;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Chestplate.Vitalis;

public class PrefixVitalis : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Chestplate;

    public LocalizedText NoSetBonus { get; private set; }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults(); // Gets the base Description safely
        NoSetBonus = LocalizationManager.GetSharedLocalizedText(LocalizationManager.NoArmorSetBonus);
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "newLine", Description.Format(Math.Round((PrefixBalance.VITALIS_LIFESTEAL_AMP - 1) * 100), 0))
        {
            IsModifier = true
        };

        yield return new TooltipLine(Mod, "newLine2", NoSetBonus.Value)
        {
            IsModifier = true,
            IsModifierBad = true
        };
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        if (player.HasBuff<ArmorAbilityCooldownBuff>()) return;
        StatPlayer statPlayer = player.GetModPlayer<StatPlayer>();
        statPlayer.AddStat(ModContent.GetInstance<HealingMulStat>(), PrefixBalance.VITALIS_LIFESTEAL_AMP, StatSource.Chestplate);
    }
}