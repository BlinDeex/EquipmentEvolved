using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.Augmented;

public class AugmentedArmorPlayer : ModPlayer
{
    public readonly Dictionary<ArmorType, AugmentedDefenseData> ArmorTypeData = new()
    {
        {
            ArmorType.Headwear,
            new AugmentedDefenseData { Thresholds = PrefixBalance.AUGMENTED_HELMET_THRESHOLDS_AND_BUFFS, CurrentDefense = 0 }
        },
        {
            ArmorType.Chestplate,
            new AugmentedDefenseData { Thresholds = PrefixBalance.AUGMENTED_CHESTPLATE_THRESHOLDS_AND_BUFFS, CurrentDefense = 0 }
        },
        {
            ArmorType.Leggings,
            new AugmentedDefenseData { Thresholds = PrefixBalance.AUGMENTED_LEGGINGS_THRESHOLDS_AND_BUFFS, CurrentDefense = 0 }
        }
    };

    public bool SetBonusActive { get; private set; }

    public override void UpdateEquips()
    {
        int augmentedPrefix = ModContent.PrefixType<PrefixAugmented>();
        int piecesEquipped = 0;

        HandleArmorDefense(Player.armor[0], ArmorType.Headwear, ref piecesEquipped, augmentedPrefix);
        HandleArmorDefense(Player.armor[1], ArmorType.Chestplate, ref piecesEquipped, augmentedPrefix);
        HandleArmorDefense(Player.armor[2], ArmorType.Leggings, ref piecesEquipped, augmentedPrefix);

        SetBonusActive = piecesEquipped == 3;

        if (SetBonusActive) Player.GetModPlayer<ArmorAbilityPlayer>().SetArmorAbility(SetBonusLogic, true);

        ApplyBuffsBasedOnDefense(ArmorType.Headwear);
        ApplyBuffsBasedOnDefense(ArmorType.Chestplate);
        ApplyBuffsBasedOnDefense(ArmorType.Leggings);
    }

    private int SetBonusLogic()
    {
        float damageIncrease = GetTotalBuffCount(Main.LocalPlayer) * PrefixBalance.AUGMENTED_SET_BONUS_DAMAGE_PER_BUFF;
        Player.GetModPlayer<StatPlayer>().DamageMul += damageIncrease;
        return 0;
    }

    private static int GetTotalBuffCount(Player player)
    {
        int count = 0;
        int activeBuffs = player.CountBuffs();

        for (int i = 0; i < activeBuffs; i++)
        {
            if (!Main.debuff[player.buffType[i]]) count++;
        }

        return count;
    }

    private void HandleArmorDefense(Item item, ArmorType armorType, ref int piecesEquipped, int augmentedPrefix)
    {
        if (item.prefix != augmentedPrefix) return;

        ArmorTypeData[armorType].CurrentDefense = item.defense;
        piecesEquipped++;
    }

    private void ApplyBuffsBasedOnDefense(ArmorType armorType)
    {
        AugmentedDefenseData armorData = ArmorTypeData[armorType];

        foreach ((int threshold, int buffID) in armorData.Thresholds)
        {
            if (armorData.CurrentDefense >= threshold) Player.AddBuff(buffID, 2);
        }
    }

    public override void ResetEffects()
    {
        ArmorTypeData[ArmorType.Headwear].CurrentDefense = 0;
        ArmorTypeData[ArmorType.Chestplate].CurrentDefense = 0;
        ArmorTypeData[ArmorType.Leggings].CurrentDefense = 0;
    }
}

public class AugmentedDefenseData
{
    public List<(int threshold, int buffID)> Thresholds { get; init; }
    public int CurrentDefense { get; set; }
}