using System;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.CharmsModule.Core;
using EquipmentEvolved.Assets.CharmsModule.Data;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.Symbiotic;

public class SymbioticPlayer : ModPlayer
{
    public int SymbioticPiecesEquipped;

    public override void ResetEffects()
    {
        SymbioticPiecesEquipped = 0;
    }

    public override void PostUpdateEquips()
    {
        bool hasFullSet = SymbioticPiecesEquipped >= 3;

        for (int i = 0; i < 3; i++)
        {
            Item armorPiece = Player.armor[i];
            if (armorPiece != null && !armorPiece.IsAir && armorPiece.TryGetGlobalItem(out CharmGlobalItem charmData))
            {
                if (hasFullSet && !charmData.IsSymbioticUpgraded && charmData.AppliedCharmRarity != CharmRarity.NotInitialized)
                    ApplySymbioticUpgrade(charmData);
                else if (!hasFullSet && charmData.IsSymbioticUpgraded) RevertSymbioticUpgrade(charmData);
            }
        }
    }

    private static void ApplySymbioticUpgrade(CharmGlobalItem itemData)
    {
        itemData.BaseSnapshot = itemData.GetCurrentData();

        CharmDataSnapshot upgradedData = itemData.BaseSnapshot.Clone();

        float qualityBoost = 0f;
        if (upgradedData.Augmentations != null)
        {
            qualityBoost = CharmBalance.SYMBIOTIC_AUGMENTATION_CONSUME_BOOST;
            upgradedData.Augmentations = null;
        }

        CharmRarity newRarity = upgradedData.Rarity + 1;
        if (newRarity > CharmRarity.Exalted) newRarity = CharmRarity.Exalted;

        upgradedData.Rarity = newRarity;

        for (int i = 0; i < upgradedData.Stats.Count; i++)
        {
            CharmRoll roll = upgradedData.Stats[i];

            // NEW: Ignore unloaded stats so the upgrade logic doesn't break
            if (roll.Stat == null || roll.IsUnloaded) continue;

            // FIXED: Using roll.Strength instead of roll.RawStrength
            float oldQuality = CharmBalance.GetRollQuality(roll.Stat, itemData.BaseSnapshot.Rarity, roll.Strength);
            float newQuality = Math.Min(oldQuality + qualityBoost, 1f);

            float newStrength = CharmBalance.CalculateStrengthFromQuality(roll.Stat, newRarity, newQuality);

            upgradedData.Stats[i] = new CharmRoll(roll.Stat, newStrength);
        }

        // FIXED: Using EquipmentStat instead of PlayerStat enum
        EquipmentStat newStat;
        int failsafe = 0;

        while (true)
        {
            failsafe++;
            newStat = CharmBalance.GetRandomStat();

            if (CharmBalance.IsCharmRareEnoughForStat(newStat, newRarity)) break;

            if (failsafe <= 1000) continue;

            UtilMethods.LogMessage("Symbiotic upgrade failed to find a valid stat.", LogType.Error);
            break;
        }

        float newStrengthStat = CharmBalance.CalculateStrengthFromQuality(newStat, newRarity, Main.rand.NextFloat());
        upgradedData.Stats.Add(new CharmRoll(newStat, newStrengthStat));

        itemData.LoadFromData(upgradedData);
        itemData.IsSymbioticUpgraded = true;
    }

    private static void RevertSymbioticUpgrade(CharmGlobalItem itemData)
    {
        itemData.LoadFromData(itemData.BaseSnapshot);

        itemData.IsSymbioticUpgraded = false;
        itemData.BaseSnapshot = null;
    }
}