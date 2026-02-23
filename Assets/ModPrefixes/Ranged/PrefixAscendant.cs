using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.InstancedGlobalItems;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged;

public class PrefixAscendant : ModPrefix, ISpecializedPrefix
{
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.RangedWeapon;
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }

    private static LocalizedText Desc { get; set; }
    
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"Ascendant", "DisplayName");


    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this,"Ascendant", nameof(Desc));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        if (!item.TryGetGlobalItem(out InstancedRangedPrefix rangedPrefix))
        {
            TooltipLine newLine = new TooltipLine(Mod, "newLine2",
                $"Could not get {nameof(InstancedRangedPrefix)}!")
            {
                OverrideColor = Color.Red
            };

            yield return newLine;
            yield break;
        }

        if (rangedPrefix.AscendantBurnOut)
        {
            TooltipLine newLine = new TooltipLine(Mod, "ascendantBurnOut",
                "This weapon has burn out and became unusable")
            {
                OverrideColor = Color.DarkGray
            };

            yield return newLine;
            yield break;
        }

        float colorLerper =
            MathHelper.Clamp(
                rangedPrefix.DamageDone / rangedPrefix.DamageDoneRequired != 0 ? rangedPrefix.DamageDoneRequired : 1, 0,
                1f);
        Color targetColor = Color.Lerp(PrefixBalance.ASCENDANT_MIN_COLOR, PrefixBalance.ASCENDANT_MAX_COLOR, colorLerper);

        if (rangedPrefix.DamageDoneRequired > 0)
        {
            TooltipLine damageDone = new TooltipLine(Mod, "damageDone",
                Desc.Format(UtilMethods.FormatNumber(rangedPrefix.DamageDone),
                    UtilMethods.FormatNumber(rangedPrefix.DamageDoneRequired)))
            {
                OverrideColor = targetColor
            };

            TooltipLine damageAdded = new TooltipLine(Mod, "damageAdded",
                SharedLocalization.GetSharedLocalizedText(SharedLocalization.XDamageAdded)
                    .Format(MathF.Round(rangedPrefix.DamageAdded * 100, 2)))
            {
                OverrideColor = targetColor
            };

            TooltipLine critAdded = new TooltipLine(Mod, "critAdded",
                SharedLocalization.GetSharedLocalizedText(SharedLocalization.XCritAdded)
                    .Format(MathF.Round(rangedPrefix.CritAdded, 2)))
            {
                OverrideColor = targetColor
            };

            yield return damageDone;
            yield return damageAdded;
            yield return critAdded;
            yield break;
        }

        targetColor = Color.DarkGray;

        TooltipLine damageDone2 = new TooltipLine(Mod, "damageDone2",
            Desc.Format("?", "?"))
        {
            OverrideColor = targetColor
        };

        TooltipLine damageAdded2 = new TooltipLine(Mod, "damageAdded2",
            SharedLocalization.GetSharedLocalizedText(SharedLocalization.XDamageAdded).Format("?"))
        {
            OverrideColor = targetColor
        };

        TooltipLine critAdded2 = new TooltipLine(Mod, "critAdded2",
            SharedLocalization.GetSharedLocalizedText(SharedLocalization.XCritAdded).Format("?"))
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