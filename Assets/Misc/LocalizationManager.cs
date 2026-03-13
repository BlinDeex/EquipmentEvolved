using System;
using System.Text;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.CharmsModule.Data;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Misc;

public static class LocalizationManager
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
    public const string XMovementSpeedIncreased = "XMovementSpeedIncreased";
    public const string XMovementSpeedDecreased = "XMovementSpeedDecreased";
    public const string XSizeIncreased = "XSizeIncreased";
    public const string XSizeDecreased = "XSizeDecreased";
    public const string XDefenseIncreased = "XDefenseIncreased";
    public const string XDefenseDecreased = "XDefenseDecreased";
    public const string XRegenIncreased = "XRegenIncreased";
    public const string XRegenDecreased = "XRegenDecreased";
    public const string XIframesIncreased = "XIframesIncreased";
    public const string XIframesDecreased = "XIframesDecreased";


    private const string weapon = "Weapon.";
    private const string armor = "Armor.";
    private const string summoner = "Summoner.";
    private const string accessory = "Accessory.";
    private const string universal = "Universal.";
    private const string multipleTools = "MultipleTools.";
    private const string tool = "Tool.";
    private const string whip = "Whip.";
    private const string minionWeapon = "MinionWeapon.";
    private const string charm = "Charms.";
    private const string common = "Common.";
    private const string rare = "Rare.";
    private const string epic = "Epic.";
    private const string legendary = "Legendary.";
    private const string mythical = "Mythical.";
    private const string notInitialized = "notInitialized";
    private const string deathReasons = "DeathReasons.";

    private static LocalizedText GetLocalizedText(string path)
    {
        return EquipmentEvolved.Instance.GetLocalization(path);
    }

    public static LocalizedText GetSharedLocalizedText(string key)
    {
        return GetLocalizedText($"LocalizationManager.{key}");
    }

    public static LocalizedText GetPrefixLocalization(ModPrefix prefix, string group = null, string locName = null)
    {
        ISpecializedPrefix specializedPrefix = prefix as ISpecializedPrefix;
        bool isSpecialized = specializedPrefix != null;
        SpecializedPrefixType specializedPrefixType = isSpecialized ? specializedPrefix.SpecializedPrefixType : SpecializedPrefixType.Empty;

        return isSpecialized ? GetSpecializedText(specializedPrefixType, group, locName) : GetNotSpecializedText(prefix, group, locName);
    }

    private static LocalizedText GetSpecializedText(SpecializedPrefixType specializedPrefixType, string group = null, string locName = null)
    {
        // --- SPECIAL CASE: Melee/Whip Hybrid Prefixes ---
        if ((specializedPrefixType & SpecializedPrefixType.MeleeWeapon) != 0 && (specializedPrefixType & SpecializedPrefixType.Whip) != 0)
        {
            specializedPrefixType = SpecializedPrefixType.MeleeWeapon;
        }
        
        StringBuilder path = new();
        
        bool isArmor = CountCommonFlags(SpecializedPrefixType.AnyArmor, specializedPrefixType) > 0;
        if (isArmor)
        {
            path.Append(armor);
            bool isUniversal = CountCommonFlags(SpecializedPrefixType.AnyArmor, specializedPrefixType) == 3;
            if (isUniversal)
            {
                path.Append(universal);
                if (group != null) path.Append(group + ".");

                path.Append(locName);
                return GetLocalizedText(path.ToString());
            }

            path.Append(specializedPrefixType + ".");
            if (group != null) path.Append(group + ".");

            path.Append(locName);
            return GetLocalizedText(path.ToString());
        }
        
        bool isTool = CountCommonFlags(specializedPrefixType, SpecializedPrefixType.AnyTool) > 0;

        if (isTool)
        {
            path.Append(tool);

            bool severalTools = CountCommonFlags(SpecializedPrefixType.AnyTool, specializedPrefixType) > 1;
            if (severalTools)
            {
                path.Append(multipleTools);
                if (group != null) path.Append(group + ".");

                path.Append(locName);
                return GetLocalizedText(path.ToString());
            }

            path.Append(specializedPrefixType + ".");
            if (group != null) path.Append(group + ".");

            path.Append(locName);
            return GetLocalizedText(path.ToString());
        }
        
        bool isSummoner = (specializedPrefixType & (SpecializedPrefixType.Whip | SpecializedPrefixType.MinionWeapon)) != 0;
        if (isSummoner)
        {
            path.Append(weapon);
            path.Append(summoner);
            path.Append(specializedPrefixType.ToString() + '.');
            if (group != null) path.Append(group + ".");

            path.Append(locName);
            return GetLocalizedText(path.ToString());
        }
        
        bool isStandardWeapon = CountCommonFlags(SpecializedPrefixType.StandardWeapons, specializedPrefixType) > 0;
        if (isStandardWeapon)
        {
            path.Append(weapon);
            path.Append(specializedPrefixType + ".");
            if (group != null) path.Append(group + ".");

            path.Append(locName);
            return GetLocalizedText(path.ToString());
        }

        throw new Exception($"GetSpecializedText reached end. Type: {specializedPrefixType}");
    }

    private static LocalizedText GetNotSpecializedText(ModPrefix prefix, string group = null, string locName = null)
    {
        StringBuilder path = new();

        PrefixCategory category = prefix.Category;

        bool isAccessory = category is PrefixCategory.Accessory;

        if (isAccessory)
        {
            path.Append(accessory);
            if (group != null) path.Append(group + ".");

            path.Append(locName);
            return GetLocalizedText(path.ToString());
        }

        bool isWeapon = category is PrefixCategory.Melee or PrefixCategory.Ranged or PrefixCategory.Magic;

        if (isWeapon)
        {
            path.Append(weapon);
            path.Append(category + ".");
            if (group != null) path.Append(group + ".");

            path.Append(locName);
            return GetLocalizedText(path.ToString());
        }

        throw new Exception("GetNotSpecializedText reached end");
    }
    
    public static LocalizedText GetCharmText(PlayerStat stat)
    {
        StringBuilder sb = new();
        sb.Append(charm);
        CharmRarity statRarity = CharmBalance.GetStatRarity(stat);
        sb.Append(statRarity switch
        {
            CharmRarity.Common => common,
            CharmRarity.Rare => rare,
            CharmRarity.Epic => epic,
            CharmRarity.Legendary => legendary,
            CharmRarity.Mythical => mythical,
            _ => notInitialized
        });
        sb.Append(stat.ToString());

        return GetLocalizedText(sb.ToString());
    }

    public static LocalizedText GetDeathReasonText(string reasonName)
    {
        return GetLocalizedText(deathReasons + reasonName);
    }
    
    private static int CountCommonFlags(Enum enum1, Enum enum2)
    {
        int commonFlags = Convert.ToInt32(enum1) & Convert.ToInt32(enum2);
        return CountSetBits(commonFlags);
    }

    private static int CountSetBits(int value)
    {
        int count = 0;
        while (value != 0)
        {
            count += value & 1;
            value >>= 1;
        }

        return count;
    }
}