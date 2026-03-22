using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Ascendant;

public class PrefixAscendant : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;
    public override float ReforgeMultiplier => PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.RangedWeapon;

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        if (!item.TryGetGlobalItem(out AscendantGlobalItem ascendantGI))
        {
            yield return new TooltipLine(Mod, "newLine2", $"Could not get {nameof(AscendantGlobalItem)}!")
            {
                IsModifier = true, IsModifierBad = true
            };
            yield break;
        }

        if (ascendantGI.AscendantBurnOut)
        {
            yield return new TooltipLine(Mod, "ascendantBurnOut", "This weapon has burn out and became unusable")
            {
                OverrideColor = Color.DarkGray
            };
            yield break;
        }

        float colorLerper = MathHelper.Clamp(ascendantGI.DamageDone / (ascendantGI.DamageDoneRequired != 0 ? ascendantGI.DamageDoneRequired : 1), 0, 1f);
        Color targetColor = Color.Lerp(PrefixBalance.INFERNAL_MIN_COLOR, PrefixBalance.INFERNAL_MAX_COLOR, colorLerper);

        if (ascendantGI.DamageDoneRequired > 0)
        {
            yield return new TooltipLine(Mod, "damageDone", Description.Format(UtilMethods.FormatNumber(ascendantGI.DamageDone), UtilMethods.FormatNumber(ascendantGI.DamageDoneRequired)))
            {
                OverrideColor = targetColor
            };

            yield return new TooltipLine(Mod, "damageAdded", LocalizationManager.GetSharedLocalizedText(LocalizationManager.XDamageAdded).Format(MathF.Round(ascendantGI.DamageAdded * 100, 2)))
            {
                OverrideColor = targetColor
            };

            yield return new TooltipLine(Mod, "critAdded", LocalizationManager.GetSharedLocalizedText(LocalizationManager.XCritAdded).Format(MathF.Round(ascendantGI.CritAdded, 2)))
            {
                OverrideColor = targetColor
            };
            
            yield break;
        }

        targetColor = Color.DarkGray;

        yield return new TooltipLine(Mod, "damageDone2", Description.Format("?", "?")) { OverrideColor = targetColor };
        yield return new TooltipLine(Mod, "damageAdded2", LocalizationManager.GetSharedLocalizedText(LocalizationManager.XDamageAdded).Format("?")) { OverrideColor = targetColor };
        yield return new TooltipLine(Mod, "critAdded2", LocalizationManager.GetSharedLocalizedText(LocalizationManager.XCritAdded).Format("?")) { OverrideColor = targetColor };
    }

    public override bool CanRoll(Item item)
    {
        return item.autoReuse;
    }
}