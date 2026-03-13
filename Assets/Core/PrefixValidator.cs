using System.Linq;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using EquipmentEvolved.Assets.ModPrefixes.Ranged.Adaptable;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Core;

public class PrefixValidator
{
    public static bool CanApplyPrefix(Item item, int prefix)
    {
        // Tools
        bool isPickaxe = item.IsPickaxe();
        bool isAxe = item.IsAxe();
        bool isHammer = item.IsHammer();
        bool isTool = item.IsTool();

        // Summoner
        bool isMinionWeapon = item.IsMinionWeapon();
        bool isWhip = item.IsWhip();

        // Armor
        bool isHeadwear = item.IsHeadwear();
        bool isChestplate = item.IsChestplate();
        bool isLeggings = item.IsLeggings();
        bool isArmor = item.IsArmor();

        // Standard Weapons 
        bool isMeleeWeapon = item.CountsAsClass(DamageClass.Melee);
        bool isRangedWeapon = item.CountsAsClass(DamageClass.Ranged);
        bool isMagicWeapon = item.CountsAsClass(DamageClass.Magic);

        PrefixConfig config = ModContent.GetInstance<PrefixConfig>();

        // Vanilla Prefix Logic
        bool vanillaPrefix = prefix < PrefixID.Count;
        if (vanillaPrefix) return !isArmor && config.EnableVanillaReforges;

        ModPrefix modPrefix = PrefixLoader.GetPrefix(prefix);
        switch (modPrefix)
        {
            case null:
            case IExperimentalPrefix when !config.EnableExperimentalReforges:
                return false;
        }

        // Handle Adaptable/Rocket Edge Case
        bool adaptablePrefix = modPrefix.Type == ModContent.PrefixType<PrefixAdaptable>();
        bool rocketWeapon = AdaptableUtils.ROCKET_WEAPON_IDS.Contains(item.type);

        if (adaptablePrefix && rocketWeapon) return false;
        
        if (modPrefix.Mod.Name != EquipmentEvolved.Instance.Name && !config.EnableOtherModPrefixes)
        {
            return false;
        }
        
        if (modPrefix is not ISpecializedPrefix specializedPrefix) return !isArmor;

        // Config checks
        if (!config.EnableModdedReforges) return false;

        if (isArmor && !config.EnableArmorReforging) return false;

        if (isTool && !config.EnableToolReforging) return false;

        SpecializedPrefixType specializedPrefixType = specializedPrefix.SpecializedPrefixType;
        
        if (isPickaxe && specializedPrefixType.HasFlag(SpecializedPrefixType.Pickaxe)) return true;

        if (isAxe && specializedPrefixType.HasFlag(SpecializedPrefixType.Axe)) return true;

        if (isHammer && specializedPrefixType.HasFlag(SpecializedPrefixType.Hammer)) return true;
        
        if (isMinionWeapon && specializedPrefixType.HasFlag(SpecializedPrefixType.MinionWeapon)) return true;

        if (isWhip && specializedPrefixType.HasFlag(SpecializedPrefixType.Whip)) return true;
        
        if (isHeadwear && specializedPrefixType.HasFlag(SpecializedPrefixType.Headwear)) return true;

        if (isChestplate && specializedPrefixType.HasFlag(SpecializedPrefixType.Chestplate)) return true;

        if (isLeggings && specializedPrefixType.HasFlag(SpecializedPrefixType.Leggings)) return true;
        
        if (isMeleeWeapon && specializedPrefixType.HasFlag(SpecializedPrefixType.MeleeWeapon)) return true;

        if (isRangedWeapon && specializedPrefixType.HasFlag(SpecializedPrefixType.RangedWeapon)) return true;

        if (isMagicWeapon && specializedPrefixType.HasFlag(SpecializedPrefixType.MagicWeapon)) return true;

        return false;
    }
}