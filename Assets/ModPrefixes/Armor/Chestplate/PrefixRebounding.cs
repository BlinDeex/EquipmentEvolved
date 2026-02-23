using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Chestplate;

public class PrefixRebounding : ModPrefix, ISpecializedPrefix
{
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Chestplate;
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public static LocalizedText Desc { get; private set; }
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this, "Rebounding", "DisplayName");

    public override void ModifyValue(ref float valueMult) => valueMult = PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    public override void SetStaticDefaults() => Desc = LocalizationManager.GetPrefixLocalization(this, "Rebounding", nameof(Desc));

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        int scalingPercent = (int)Math.Round(PrefixBalance.REBOUNDING_DEFENSE_SCALING * 100);
        yield return new TooltipLine(Mod, "ReboundingDesc", Desc.Format(scalingPercent, PrefixBalance.REBOUNDING_MAX_STACKS)) { IsModifier = true };
    }
}