using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Core;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.Augmented;

public class PrefixAugmented : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;

    public static LocalizedText SetBonus { get; private set; }
    public static LocalizedText Desc { get; private set; }

    public static LocalizedText HeadwearLvl1 { get; private set; }
    public static LocalizedText HeadwearLvl2 { get; private set; }
    public static LocalizedText HeadwearLvl3 { get; private set; }

    public static LocalizedText ChestplateLvl1 { get; private set; }
    public static LocalizedText ChestplateLvl2 { get; private set; }
    public static LocalizedText ChestplateLvl3 { get; private set; }

    public static LocalizedText LeggingsLvl1 { get; private set; }
    public static LocalizedText LeggingsLvl2 { get; private set; }
    public static LocalizedText LeggingsLvl3 { get; private set; }

    public static Dictionary<ArmorType, LocalizedText[]> ArmorLevels { get; private set; }

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Augmented", "DisplayName");

    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Headwear | SpecializedPrefixType.Chestplate | SpecializedPrefixType.Leggings;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    }

    public override void SetStaticDefaults()
    {
        SetBonus = LocalizationManager.GetPrefixLocalization(this, "Augmented", nameof(SetBonus));
        Desc = LocalizationManager.GetPrefixLocalization(this, "Augmented", nameof(Desc));

        ArmorLevels = new Dictionary<ArmorType, LocalizedText[]>
        {
            {
                ArmorType.Headwear, [
                    LocalizationManager.GetPrefixLocalization(this, "Augmented", nameof(HeadwearLvl1)),
                    LocalizationManager.GetPrefixLocalization(this, "Augmented", nameof(HeadwearLvl2)),
                    LocalizationManager.GetPrefixLocalization(this, "Augmented", nameof(HeadwearLvl3))
                ]
            },
            {
                ArmorType.Chestplate, [
                    LocalizationManager.GetPrefixLocalization(this, "Augmented", nameof(ChestplateLvl1)),
                    LocalizationManager.GetPrefixLocalization(this, "Augmented", nameof(ChestplateLvl2)),
                    LocalizationManager.GetPrefixLocalization(this, "Augmented", nameof(ChestplateLvl3))
                ]
            },
            {
                ArmorType.Leggings, [
                    LocalizationManager.GetPrefixLocalization(this, "Augmented", nameof(LeggingsLvl1)),
                    LocalizationManager.GetPrefixLocalization(this, "Augmented", nameof(LeggingsLvl2)),
                    LocalizationManager.GetPrefixLocalization(this, "Augmented", nameof(LeggingsLvl3))
                ]
            }
        };
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        AugmentedArmorPlayer augmentedArmorPlayer = Main.LocalPlayer.GetModPlayer<AugmentedArmorPlayer>();

        TooltipLine desc = new(Mod, "desc", Desc.Value)
        {
            IsModifier = true,
            IsModifierBad = !augmentedArmorPlayer.SetBonusActive
        };

        yield return desc;

        if (item.IsHeadwear())
        {
            foreach (TooltipLine tooltip in GenerateTooltipLines(ArmorType.Headwear, augmentedArmorPlayer.ArmorTypeData[ArmorType.Headwear].CurrentDefense))
            {
                yield return tooltip;
            }
        }
        else if (item.IsChestplate())
        {
            foreach (TooltipLine tooltip in GenerateTooltipLines(ArmorType.Chestplate, augmentedArmorPlayer.ArmorTypeData[ArmorType.Chestplate].CurrentDefense))
            {
                yield return tooltip;
            }
        }
        else if (item.IsLeggings())
        {
            foreach (TooltipLine tooltip in GenerateTooltipLines(ArmorType.Leggings, augmentedArmorPlayer.ArmorTypeData[ArmorType.Leggings].CurrentDefense))
            {
                yield return tooltip;
            }
        }

        string buffBonus = " [0]%";
        int armorCDBuff = ModContent.BuffType<ArmorAbilityCooldownBuff>();
        bool playerHasArmorCD = Main.LocalPlayer.HasBuff(armorCDBuff);

        if (augmentedArmorPlayer.SetBonusActive && !playerHasArmorCD)
        {
            float damageIncrease = GetTotalBuffCount(Main.LocalPlayer) * PrefixBalance.AUGMENTED_SET_BONUS_DAMAGE_PER_BUFF;

            damageIncrease = MathF.Round(damageIncrease * 100f, 2);

            buffBonus = $" [{damageIncrease}]%";
        }

        TooltipLine setBonus = new(Mod, "setBonus", SetBonus.Format(Math.Round(PrefixBalance.AUGMENTED_SET_BONUS_DAMAGE_PER_BUFF * 100, 2)) + buffBonus)
        {
            IsModifier = true,
            IsModifierBad = !augmentedArmorPlayer.SetBonusActive || playerHasArmorCD
        };

        yield return setBonus;
    }

    private static int GetTotalBuffCount(Player player)
    {
        int count = 0;
        int i = 0;
        int max = player.CountBuffs();

        while (i != max)
        {
            i++;
            if (Main.debuff[player.buffType[i]]) continue;

            count++;
        }

        return count;
    }

    private IEnumerable<TooltipLine> GenerateTooltipLines(ArmorType armorType, int defenseValue)
    {
        List<(int threshold, int buffID)> thresholds = armorType switch
        {
            ArmorType.Headwear => PrefixBalance.AUGMENTED_HELMET_THRESHOLDS_AND_BUFFS,
            ArmorType.Chestplate => PrefixBalance.AUGMENTED_CHESTPLATE_THRESHOLDS_AND_BUFFS,
            _ => PrefixBalance.AUGMENTED_LEGGINGS_THRESHOLDS_AND_BUFFS
        };

        LocalizedText[] localizedLevels = ArmorLevels[armorType];

        for (int i = 0; i < thresholds.Count; i++)
        {
            yield return new TooltipLine(Mod, $"line{i + 1}", localizedLevels[i].Format(thresholds[i].threshold))
            {
                OverrideColor = defenseValue >= thresholds[i].threshold ? PrefixBalance.AUGMENTED_TOOLTIP_COLORS[i] : PrefixBalance.AUGMENTED_RED_COLOR
            };
        }
    }
}