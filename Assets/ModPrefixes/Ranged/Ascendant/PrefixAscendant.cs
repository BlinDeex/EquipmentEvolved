using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Ascendant;

public class PrefixAscendant : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;

    private static LocalizedText Desc { get; set; }

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Ascendant", "DisplayName");

    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.RangedWeapon;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }


    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this, "Ascendant", nameof(Desc));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        if (!item.TryGetGlobalItem(out AscendantGlobalItem ascendantGI))
        {
            TooltipLine newLine = new(Mod, "newLine2", $"Could not get {nameof(AscendantGlobalItem)}!")
            {
                OverrideColor = Color.Red
            };

            yield return newLine;
            yield break;
        }

        if (ascendantGI.AscendantBurnOut)
        {
            TooltipLine newLine = new(Mod, "ascendantBurnOut", "This weapon has burn out and became unusable")
            {
                OverrideColor = Color.DarkGray
            };

            yield return newLine;
            yield break;
        }

        float colorLerper = MathHelper.Clamp(ascendantGI.DamageDone / ascendantGI.DamageDoneRequired != 0 ? ascendantGI.DamageDoneRequired : 1, 0, 1f);
        Color targetColor = Color.Lerp(PrefixBalance.INFERNAL_MIN_COLOR, PrefixBalance.INFERNAL_MAX_COLOR, colorLerper);

        if (ascendantGI.DamageDoneRequired > 0)
        {
            TooltipLine damageDone = new(Mod, "damageDone", Desc.Format(UtilMethods.FormatNumber(ascendantGI.DamageDone), UtilMethods.FormatNumber(ascendantGI.DamageDoneRequired)))
            {
                OverrideColor = targetColor
            };

            TooltipLine damageAdded = new(Mod, "damageAdded", LocalizationManager.GetSharedLocalizedText(LocalizationManager.XDamageAdded).Format(MathF.Round(ascendantGI.DamageAdded * 100, 2)))
            {
                OverrideColor = targetColor
            };

            TooltipLine critAdded = new(Mod, "critAdded", LocalizationManager.GetSharedLocalizedText(LocalizationManager.XCritAdded).Format(MathF.Round(ascendantGI.CritAdded, 2)))
            {
                OverrideColor = targetColor
            };

            yield return damageDone;
            yield return damageAdded;
            yield return critAdded;
            yield break;
        }

        targetColor = Color.DarkGray;

        TooltipLine damageDone2 = new(Mod, "damageDone2", Desc.Format("?", "?"))
        {
            OverrideColor = targetColor
        };

        TooltipLine damageAdded2 = new(Mod, "damageAdded2", LocalizationManager.GetSharedLocalizedText(LocalizationManager.XDamageAdded).Format("?"))
        {
            OverrideColor = targetColor
        };

        TooltipLine critAdded2 = new(Mod, "critAdded2", LocalizationManager.GetSharedLocalizedText(LocalizationManager.XCritAdded).Format("?"))
        {
            OverrideColor = targetColor
        };

        yield return damageDone2;
        yield return damageAdded2;
        yield return critAdded2;
    }

    public override bool CanRoll(Item item)
    {
        return item.autoReuse;
    }
}