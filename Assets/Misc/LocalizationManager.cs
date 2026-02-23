using System;
using System.Text;
using EquipmentEvolved.Assets.CharmsModule;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Misc;

public static class LocalizationManager
{
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

    private const SpecializedPrefixType ARMOR_FLAGS = SpecializedPrefixType.Headwear | SpecializedPrefixType.Chestplate | SpecializedPrefixType.Leggings;

    private const SpecializedPrefixType TOOL_FLAGS = SpecializedPrefixType.Pickaxe | SpecializedPrefixType.Axe | SpecializedPrefixType.Hammer;

    // Added the new standard weapon flags here for easy checking
    private const SpecializedPrefixType STANDARD_WEAPON_FLAGS = SpecializedPrefixType.MeleeWeapon | SpecializedPrefixType.RangedWeapon | SpecializedPrefixType.MagicWeapon;

    public static LocalizedText GetPrefixLocalization(ModPrefix prefix, string group = null, string locName = null)
    {
        ISpecializedPrefix specializedPrefix = prefix as ISpecializedPrefix;
        bool isSpecialized = specializedPrefix != null;
        SpecializedPrefixType specializedPrefixType = isSpecialized ? specializedPrefix.SpecializedPrefixType : SpecializedPrefixType.Empty;

        return isSpecialized ? GetSpecializedText(specializedPrefixType, group, locName) : GetNotSpecializedText(prefix, group, locName);
    }

    /// <summary>
    /// Automatically builds path and returns localized text
    /// </summary>
    private static LocalizedText GetSpecializedText(SpecializedPrefixType specializedPrefixType, string group = null, string locName = null)
    {
        StringBuilder path = new();

        // 1. Check Armor
        bool isArmor = CountCommonFlags(ARMOR_FLAGS, specializedPrefixType) > 0;
        if (isArmor)
        {
            path.Append(armor);
            bool isUniversal = CountCommonFlags(ARMOR_FLAGS, specializedPrefixType) == 3;
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

        // 2. Check Tools
        bool isTool = CountCommonFlags(specializedPrefixType, TOOL_FLAGS) > 0;
        if (isTool)
        {
            path.Append(tool);

            bool severalTools = CountCommonFlags(TOOL_FLAGS, specializedPrefixType) > 1;
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

        // 3. Check Summoner
        // Note: Updated this check slightly to handle combined flags or direct matching more robustly
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

        // 4. Check Standard Weapons (FIX)
        // This catches MeleeWeapon, RangedWeapon, MagicWeapon
        bool isStandardWeapon = CountCommonFlags(STANDARD_WEAPON_FLAGS, specializedPrefixType) > 0;
        if (isStandardWeapon)
        {
            path.Append(weapon); // "Weapon."
            path.Append(specializedPrefixType.ToString() + "."); // e.g. "MeleeWeapon."
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

    public static LocalizedText GetCharmText(CharmStat stat)
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

        return EquipmentEvolved.Instance.GetLocalization(sb.ToString());
    }

    private static LocalizedText GetLocalizedText(string path) => EquipmentEvolved.Instance.GetLocalization(path);

    private static int CountCommonFlags(Enum enum1, Enum enum2)
    {
        int commonFlags = Convert.ToInt32(enum1) & Convert.ToInt32(enum2);
        
        int commonFlagCount = CountSetBits(commonFlags);
        
        return commonFlagCount;
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