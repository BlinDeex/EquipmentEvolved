using System.Collections.Generic;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Misc;

public static class SharedLocalization
{
    public const string PrefixAscendant = "AscendantDesc";
    public const string XDamageAdded = "XDamageAdded";
    public const string XDamageDecreased = "XDamageDecreased";
    public const string XCritAdded = "XCritAdded";
    public const string XCritDecreased = "XCritDecreased";
    public const string XDecreasedManaUsage = "XDecreasedManaUsage";
    public const string XIncreasedManaUsage = "XIncreasedManaUsage";
    public const string XUseTimeReduced = "XUseTimeReduced";
    public const string XUseTimeIncreased = "XUseTimeIncreased";
    public const string XCritDamageIncreased = "XCritDamageIncreased";
    public const string XCritDamageDecreased = "XCritDamageDecreased";
    public const string XOnHitIncreasedCoinDropChance = "XOnHitIncreasedCoinDropChance";
    public const string XOnHitDecreasedCoinDropChance = "XOnHitDecreasedCoinDropChance";
    public const string XIncreasedCoinDropValue = "XIncreasedCoinDropValue";
    public const string XDecreasedCoinDropValue = "XDecreasedCoinDropValue";
    public const string XIncreasedLifesteal = "XIncreasedLifesteal";
    public const string XDecreasedLifesteal = "XDecreasedLifesteal";
    public const string XNegativeMaxHealthDamage = "XNegativeMaxHealthDamage";
    public const string XPositiveMaxHealthDamage = "XPositiveMaxHealthDamage";
    public const string NoArmorSetBonus = "NoArmorSetBonus";

    private static readonly Dictionary<string, LocalizedText> SharedLocalizedTexts = new();

    public static void Load()
    {
        Mod mod = EquipmentEvolved.Instance;
        SharedLocalizedTexts.Add(PrefixAscendant, mod.GetLocalization($"SharedLocalization.{PrefixAscendant}"));
        SharedLocalizedTexts.Add(XDamageAdded, mod.GetLocalization($"SharedLocalization.{XDamageAdded}"));
        SharedLocalizedTexts.Add(XDamageDecreased, mod.GetLocalization($"SharedLocalization.{XDamageDecreased}"));
        SharedLocalizedTexts.Add(XCritAdded, mod.GetLocalization($"SharedLocalization.{XCritAdded}"));
        SharedLocalizedTexts.Add(XCritDecreased, mod.GetLocalization($"SharedLocalization.{XCritDecreased}"));
        SharedLocalizedTexts.Add(XDecreasedManaUsage, mod.GetLocalization($"SharedLocalization.{XDecreasedManaUsage}"));
        SharedLocalizedTexts.Add(XIncreasedManaUsage, mod.GetLocalization($"SharedLocalization.{XIncreasedManaUsage}"));
        SharedLocalizedTexts.Add(XUseTimeReduced, mod.GetLocalization($"SharedLocalization.{XUseTimeReduced}"));
        SharedLocalizedTexts.Add(XUseTimeIncreased, mod.GetLocalization($"SharedLocalization.{XUseTimeIncreased}"));
        SharedLocalizedTexts.Add(XCritDamageIncreased, mod.GetLocalization($"SharedLocalization.{XCritDamageIncreased}"));
        SharedLocalizedTexts.Add(XCritDamageDecreased, mod.GetLocalization($"SharedLocalization.{XCritDamageDecreased}"));
        SharedLocalizedTexts.Add(XOnHitIncreasedCoinDropChance, mod.GetLocalization($"SharedLocalization.{XOnHitIncreasedCoinDropChance}"));
        SharedLocalizedTexts.Add(XOnHitDecreasedCoinDropChance, mod.GetLocalization($"SharedLocalization.{XOnHitDecreasedCoinDropChance}"));
        SharedLocalizedTexts.Add(XIncreasedCoinDropValue, mod.GetLocalization($"SharedLocalization.{XIncreasedCoinDropValue}"));
        SharedLocalizedTexts.Add(XDecreasedCoinDropValue, mod.GetLocalization($"SharedLocalization.{XDecreasedCoinDropValue}"));
        SharedLocalizedTexts.Add(XIncreasedLifesteal, mod.GetLocalization($"SharedLocalization.{XIncreasedLifesteal}"));
        SharedLocalizedTexts.Add(XDecreasedLifesteal, mod.GetLocalization($"SharedLocalization.{XDecreasedLifesteal}"));
        SharedLocalizedTexts.Add(XNegativeMaxHealthDamage, mod.GetLocalization($"SharedLocalization.{XNegativeMaxHealthDamage}"));
        SharedLocalizedTexts.Add(XPositiveMaxHealthDamage, mod.GetLocalization($"SharedLocalization.{XPositiveMaxHealthDamage}"));
        SharedLocalizedTexts.Add(NoArmorSetBonus, mod.GetLocalization($"SharedLocalization.{NoArmorSetBonus}"));
    }


    public static LocalizedText GetSharedLocalizedText(string key)
    {
        return SharedLocalizedTexts[key];
    }
}