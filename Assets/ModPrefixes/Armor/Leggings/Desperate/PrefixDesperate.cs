using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Leggings.Desperate;

public class PrefixDesperate : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;

    public static LocalizedText Desc { get; private set; }

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Desperate", "DisplayName");

    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Leggings;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    }

    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this, "Desperate", nameof(Desc));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        int speedPercent = (int)Math.Round(PrefixBalance.DESPERATE_MAX_SPEED_BONUS * 100);

        yield return new TooltipLine(Mod, "DesperateDesc", Desc.Format(speedPercent))
        {
            IsModifier = true,
            IsModifierBad = false
        };
    }
}