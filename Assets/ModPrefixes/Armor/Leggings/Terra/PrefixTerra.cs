using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Leggings.Terra;

public class PrefixTerra : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;

    public static LocalizedText Desc { get; private set; }

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Terra", "DisplayName");

    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Leggings;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    }

    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this, "Terra", nameof(Desc));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        int defensePercent = (int)Math.Round(PrefixBalance.TERRA_DEFENSE_MULT * 100);

        yield return new TooltipLine(Mod, "TerraDesc", Desc.Format(defensePercent))
        {
            IsModifier = true,
            IsModifierBad = false
        };
    }
}