using System.Linq;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Buffs;
using EquipmentEvolved.Assets.InstancedGlobalItems;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPlayers.Armor.Universal;
using EquipmentEvolved.Assets.ModPrefixes.Magic;
using EquipmentEvolved.Assets.ModPrefixes.Ranged;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace EquipmentEvolved.Assets.Detours;

public class Detours : ModSystem
{
    private static On_Item.orig_CanApplyPrefix origCanApplyPrefix;
    
    public override void SetupContent()
    {
        On_Item.RollAPrefix += OnRollAPrefix;
        On_Player.AddBuff += OnAddBuff;
        On_Projectile.AI += ProjectileOnAI;
        On_Item.GetVanillaPrefixes += (_, _) => [];
    }

    public override void PostSetupContent()
    {
        On_Item.CanApplyPrefix += (orig, self, prefix) =>
        {
            origCanApplyPrefix = orig;
            return CanApplyPrefix(self, prefix);
        };
        
        On_Player.CheckDrowning += (orig, self) =>
        {
            self.breath = self.GetModPlayer<MiscArmorPlayer>().WaterBreathingPrefix();
            orig.Invoke(self);
        };
    }

    private void ProjectileOnAI(On_Projectile.orig_AI orig, Projectile self)
    {
        if (!self.TryGetGlobalProjectile(out InstancedProjectilePrefix projPrefix) || !projPrefix.Frenzied)
        {
            orig.Invoke(self);
            return;
        }

        for (int i = 0; i < PrefixBalance.FRENZIED_ADDITIONAL_UPDATES; i++) orig.Invoke(self);
    }

    private void OnAddBuff(On_Player.orig_AddBuff orig, Player self, int type, int timeToAdd, bool quiet, bool foodHack)
    {
        if (self.HeldItem.prefix == ModContent.PrefixType<PrefixInverted>() && type == BuffID.ManaSickness)
        {
            orig.Invoke(self, ModContent.BuffType<InvertedBuff>(), timeToAdd, quiet, foodHack);
            return;
        }

        orig.Invoke(self, type, timeToAdd, quiet, foodHack);
    }

    private bool OnRollAPrefix(On_Item.orig_RollAPrefix orig, Item self, UnifiedRandom random, ref int rolledPrefix)
    {
        // 1. Check if a specific prefix is forced by another mod (Standard tModLoader behavior)
        int forcedPrefix = ItemLoader.ChoosePrefix(self, random);
        if (forcedPrefix > 0 && CanApplyPrefix(self, forcedPrefix))
        {
            rolledPrefix = forcedPrefix;
            return true;
        }

        // 2. Build a pool of ALL valid prefixes (Vanilla + Modded)
        // This replaces the loop 1000 approach with a deterministic weighted scan.
        var validPrefixes = new WeightedRandom<int>(random);

        for (int i = 1; i < PrefixLoader.PrefixCount; i++)
        {
            // We verify both our custom rules (CanApplyPrefix) AND other mods' potential vetoes (AllowPrefix)
            if (CanApplyPrefix(self, i) && ItemLoader.AllowPrefix(self, i))
            {
                double weight = 1.0;

                // For modded prefixes, respect their defined RollChance
                if (i >= PrefixID.Count)
                {
                    ModPrefix modPrefix = PrefixLoader.GetPrefix(i);
                    if (modPrefix != null)
                    {
                        weight = modPrefix.RollChance(self);
                    }
                }

                validPrefixes.Add(i, weight);
            }
        }

        // 3. Select a prefix from the valid pool
        if (validPrefixes.elements.Count > 0)
        {
            rolledPrefix = validPrefixes.Get();
            return true;
        }

        // If no prefixes are valid, return false (Reforge fails / Item loses prefix)
        return false;
    }

    public static bool CanApplyPrefix(Item item, int prefix)
    {
        // 1. Burnout Check
        if (item.GetGlobalItem<InstancedRangedPrefix>().AscendantBurnOut) return false;

        // --- Helper Booleans ---
        // Tools
        bool isPickaxe = item.IsPickaxe();
        bool isAxe = item.IsAxe();
        bool isHammer = item.IsHammer();

        // Summoner
        bool isMinionWeapon = item.IsMinionWeapon();
        bool isWhip = item.IsWhip();

        // Armor
        bool isHeadwear = item.IsHeadwear();
        bool isChestplate = item.IsChestplate();
        bool isLeggings = item.IsLeggings();
        bool isArmor = isHeadwear || isChestplate || isLeggings;

        // Standard Weapons (UPDATED: We now check DamageClass directly so tools aren't excluded)
        bool isMeleeWeapon = item.CountsAsClass(DamageClass.Melee);
        bool isRangedWeapon = item.CountsAsClass(DamageClass.Ranged);
        bool isMagicWeapon = item.CountsAsClass(DamageClass.Magic);

        // 2. Vanilla Prefix Logic
        bool vanillaPrefix = prefix < PrefixID.Count;
        if (vanillaPrefix)
        {
            if (isArmor) return false;

            bool allowVanillaPrefixes = ModContent.GetInstance<PrefixConfig>().AllowVanillaPrefixes;
            return allowVanillaPrefixes && origCanApplyPrefix.Invoke(item, prefix);
        }

        ModPrefix modPrefix = PrefixLoader.GetPrefix(prefix);
        if (modPrefix == null) return false;

        // 3. Handle Adaptable/Rocket Edge Case
        bool adaptablePrefix = modPrefix.Type == ModContent.PrefixType<PrefixAdaptable>();
        bool rocketWeapon = AdaptableUtils.ROCKET_WEAPON_IDS.Contains(item.type);

        if (adaptablePrefix && rocketWeapon)
        {
            return false;
        }

        // 4. Handle Generic Modded Prefixes
        if (modPrefix is not ISpecializedPrefix specializedPrefix)
        {
            return !isArmor && origCanApplyPrefix.Invoke(item, prefix);
        }

        SpecializedPrefixType specializedPrefixType = specializedPrefix.SpecializedPrefixType;

        // 5. Match Item Type to Specialized Flags
        // Tools
        if (isPickaxe && specializedPrefixType.HasFlag(SpecializedPrefixType.Pickaxe)) return true;
        if (isAxe && specializedPrefixType.HasFlag(SpecializedPrefixType.Axe)) return true;
        if (isHammer && specializedPrefixType.HasFlag(SpecializedPrefixType.Hammer)) return true;

        // Summoner
        if (isMinionWeapon && specializedPrefixType.HasFlag(SpecializedPrefixType.MinionWeapon)) return true;
        if (isWhip && specializedPrefixType.HasFlag(SpecializedPrefixType.Whip)) return true;

        // Armor
        if (isHeadwear && specializedPrefixType.HasFlag(SpecializedPrefixType.Headwear)) return true;
        if (isChestplate && specializedPrefixType.HasFlag(SpecializedPrefixType.Chestplate)) return true;
        if (isLeggings && specializedPrefixType.HasFlag(SpecializedPrefixType.Leggings)) return true;

        // Standard Weapons
        if (isMeleeWeapon && specializedPrefixType.HasFlag(SpecializedPrefixType.MeleeWeapon)) return true;
        if (isRangedWeapon && specializedPrefixType.HasFlag(SpecializedPrefixType.RangedWeapon)) return true;
        if (isMagicWeapon && specializedPrefixType.HasFlag(SpecializedPrefixType.MagicWeapon)) return true;

        return false;
    }
}