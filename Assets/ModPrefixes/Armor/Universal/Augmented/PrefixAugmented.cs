using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Core;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.Augmented;

public class PrefixAugmented : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Headwear | SpecializedPrefixType.Chestplate | SpecializedPrefixType.Leggings;

    public LocalizedText SetBonus { get; private set; }
    public LocalizedText LevelLine { get; private set; }

    protected override void OnSetStaticDefaults()
    {
         
        SetBonus = GetLoc(nameof(SetBonus));
        LevelLine = GetLoc(nameof(LevelLine));
    }

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        AugmentedArmorPlayer augmentedArmorPlayer = Main.LocalPlayer.GetModPlayer<AugmentedArmorPlayer>();

        TooltipLine desc = new(Mod, "desc", Description.Value)
        {
            IsModifier = true,
            IsModifierBad = !augmentedArmorPlayer.SetBonusActive
        };

        yield return desc;

        if (item.IsHeadwear())
        {
            foreach (TooltipLine tooltip in GenerateTooltipLines(ArmorType.Headwear, augmentedArmorPlayer.ArmorTypeData[ArmorType.Headwear].CurrentDefense))
                yield return tooltip;
        }
        else if (item.IsChestplate())
        {
            foreach (TooltipLine tooltip in GenerateTooltipLines(ArmorType.Chestplate, augmentedArmorPlayer.ArmorTypeData[ArmorType.Chestplate].CurrentDefense))
                yield return tooltip;
        }
        else if (item.IsLeggings())
        {
            foreach (TooltipLine tooltip in GenerateTooltipLines(ArmorType.Leggings, augmentedArmorPlayer.ArmorTypeData[ArmorType.Leggings].CurrentDefense))
                yield return tooltip;
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

        for (int i = 0; i < thresholds.Count; i++)
        {
            string buffName = Lang.GetBuffName(thresholds[i].buffID);
            yield return new TooltipLine(Mod, $"line{i + 1}", LevelLine.Format(thresholds[i].threshold, buffName))
            {
                OverrideColor = defenseValue >= thresholds[i].threshold ? PrefixBalance.AUGMENTED_TOOLTIP_COLORS[i] : PrefixBalance.AUGMENTED_RED_COLOR
            };
        }
    }
}