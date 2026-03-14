using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using EquipmentEvolved.Assets.CharmsModule.Data;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.ModPrefixes.Ranged.Challenger;
using EquipmentEvolved.Assets.Stats.Combat;
using EquipmentEvolved.Assets.Stats.Custom;
using EquipmentEvolved.Assets.Stats.Defense;
using EquipmentEvolved.Assets.Stats.MobilityUtility;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Balance;

[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
[SuppressMessage("ReSharper", "ConvertToConstant.Global")]
[SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible")]
public static class CharmBalance
{
    #region 1. General Drops & Roll Tries

    public static int RollTriesForBosses = 10;

    public static readonly Dictionary<CharmRarity, (int min, int max)> StatRollBounds = new()
    {
        { CharmRarity.Common, (1, 3) },
        { CharmRarity.Rare, (2, 4) },
        { CharmRarity.Epic, (3, 5) },
        { CharmRarity.Legendary, (4, 6) },
        { CharmRarity.Mythical, (5, 7) },
        { CharmRarity.Exalted, (10, 10) }
    };

    public static int LegendaryPity = 250;
    public static int MythicalPity = 1000;

    public static List<int> ExcludedNPCSFromCharmDrops = 
    [
        ModContent.NPCType<ChallengerOrb>(),
        NPCID.EaterofWorldsBody
    ];

    public static Dictionary<float, CharmType> CharmTypeChance = new()
    {
        { 0.33f, CharmType.Circle },
        { 0.33f + 0.33f, CharmType.Square },
        { 1f, CharmType.Triangle }
    };

    public static List<(float chance, CharmRarity rarity)> CharmRarityDropTable =
    [
        (0.001f, CharmRarity.Mythical), // 0.1% Absolute chance
        (0.005f, CharmRarity.Legendary), // 0.4% Absolute chance 
        (0.015f, CharmRarity.Epic), // 1.0% Absolute chance 
        (0.040f, CharmRarity.Rare), // 2.5% Absolute chance 
        (0.100f, CharmRarity.Common)
    ];

    #endregion

    #region 2. Augmentation & System Balance

    public static float OMNI_AUGMENTATION_CHANCE = 0.1f;
    public static float LEGENDARY_AUGMENTATION_CHANCE = 0.33f;
    public const float SYMBIOTIC_BASE_QUALITY_BOOST = 0.10f; // +10% quality when applying a charm
    public const float SYMBIOTIC_AUGMENTATION_CONSUME_BOOST = 0.20f; // +20% quality from consuming an augmentation

    public static readonly Dictionary<CharmRarity, float[]> AUGMENTATION_CHANCES = new()
    {
        { CharmRarity.Common, [] },
        { CharmRarity.Rare, [0.05f] },
        { CharmRarity.Epic, [0.20f] },
        { CharmRarity.Legendary, [0.50f, 0.05f] },
        { CharmRarity.Mythical, [1.00f, 0.25f] },
        { CharmRarity.Exalted, [1.00f, 1.00f] }
    };

    #endregion

    #region 3. Stat Bounds & Multipliers

    // Lazy-loaded to prevent ModContent.GetInstance from throwing errors during server startup
    private static Dictionary<EquipmentStat, (CharmRarity minRarity, float min, float max)> _statDefinitions;

    public static Dictionary<EquipmentStat, (CharmRarity minRarity, float min, float max)> StatDefinitions
    {
        get
        {
            if (_statDefinitions == null)
            {
                _statDefinitions = new()
                {
                    // --- Offensive Stats ---
                    { ModContent.GetInstance<DamageStat>(), (CharmRarity.Common, 0.01f, 0.02f) },
                    { ModContent.GetInstance<MeleeDamageStat>(), (CharmRarity.Common, 0.015f, 0.03f) },
                    { ModContent.GetInstance<RangedDamageStat>(), (CharmRarity.Common, 0.015f, 0.03f) },
                    { ModContent.GetInstance<MagicDamageStat>(), (CharmRarity.Common, 0.015f, 0.03f) },
                    { ModContent.GetInstance<SummonDamageStat>(), (CharmRarity.Common, 0.015f, 0.03f) },
                    { ModContent.GetInstance<CritStat>(), (CharmRarity.Common, 1f, 2f) },
                    { ModContent.GetInstance<CritDamageStat>(), (CharmRarity.Epic, 0.02f, 0.05f) },
                    { ModContent.GetInstance<TrueDamageMulStat>(), (CharmRarity.Legendary, 0.02f, 0.04f) },

                    // --- Utility Stats ---
                    { ModContent.GetInstance<UseSpeedStat>(), (CharmRarity.Rare, 0.01f, 0.02f) },
                    { ModContent.GetInstance<HealthCapMulStat>(), (CharmRarity.Rare, -0.02f, -0.03f) },
                    { ModContent.GetInstance<PickSpeedStat>(), (CharmRarity.Common, 0.02f, 0.05f) },
                    { ModContent.GetInstance<MoveSpeedStat>(), (CharmRarity.Common, 0.01f, 0.02f) },
                    { ModContent.GetInstance<WingTimeStat>(), (CharmRarity.Rare, 5f, 15f) },
                    { ModContent.GetInstance<ManaUsageStat>(), (CharmRarity.Rare, -0.01f, -0.03f) },
                    { ModContent.GetInstance<CharmLuckStat>(), (CharmRarity.Common, 0.01f, 0.05f) },

                    // --- Survivability (Highly Regulated) ---
                    { ModContent.GetInstance<MaxHealthMulStat>(), (CharmRarity.Epic, 0.01f, 0.02f) },
                    { ModContent.GetInstance<RegenStat>(), (CharmRarity.Rare, 0.25f, 0.75f) },
                    { ModContent.GetInstance<HealingMulStat>(), (CharmRarity.Epic, 0.01f, 0.03f) },
                    { ModContent.GetInstance<LifeStealStat>(), (CharmRarity.Legendary, 0.008f, 0.016f) },
                    { ModContent.GetInstance<DamageLifestealStat>(), (CharmRarity.Epic, 0.01f, 0.03f) },
                    { ModContent.GetInstance<DamageReductionStat>(), (CharmRarity.Mythical, 0.01f, 0.02f) },
                    { ModContent.GetInstance<IframesStat>(), (CharmRarity.Legendary, 1.0f, 3.0f) } 
                };
            }
            return _statDefinitions;
        }
    }

    public static EquipmentStat[] ValidCharmStats => StatDefinitions.Keys.ToArray();

    private static readonly Dictionary<CharmRarity, float> statRarityMultipliers = new()
    {
        { CharmRarity.NotInitialized, 0f },
        { CharmRarity.Common, 1f },
        { CharmRarity.Rare, 1.5f },
        { CharmRarity.Epic, 2f },
        { CharmRarity.Legendary, 3f },
        { CharmRarity.Mythical, 4f },
        { CharmRarity.Exalted, 5f }
    };

    #endregion

    #region 4. Visuals & Colors

    private static readonly Dictionary<CharmRarity, Func<Color>> CharmColors = new()
    {
        { CharmRarity.NotInitialized, () => new Color(255, 255, 255) },
        { CharmRarity.Common, () => new Color(255, 255, 255) },
        { CharmRarity.Rare, () => new Color(100, 100, 255) },
        { CharmRarity.Epic, () => new Color(255, 0, 255) },
        { CharmRarity.Legendary, () => new Color(255, 255, 0) },
        { CharmRarity.Mythical, () => new Color(255, 100, 100) },
        {
            CharmRarity.Exalted, () =>
            {
                float pulse = (MathF.Sin(Main.GlobalTimeWrappedHourly * 3f) + 1f) / 2f;
                return Color.Lerp(Main.DiscoColor, Color.White, 0.2f + 0.6f * pulse);
            }
        }
    };

    public static Color GetCharmColor(CharmRarity rarity)
    {
        if (CharmColors.TryGetValue(rarity, out Func<Color> color)) return color.Invoke();

        UtilMethods.BroadcastOrNewText($"{nameof(GetCharmColor)}: {nameof(CharmColors)} didnt contain rarity of {rarity.ToString()}!", Color.Red);
        return Color.Black;
    }

    public static Color GetStatColor(EquipmentStat stat)
    {
        if (stat == null) return Color.Gray; // Color for Unloaded stats!
        
        CharmRarity rarity = GetStatRarity(stat);
        return GetCharmColor(rarity);
    }

    #endregion

    #region 5. Mathematics & Roll Logic

    public static EquipmentStat GetRandomStat()
    {
        return ValidCharmStats[Main.rand.Next(ValidCharmStats.Length)];
    }

    /// <summary>
    ///     Calculates how perfect a charm stat roll is, returning a value from 0.00 to 100.00
    /// </summary>
    public static float GetRollQualityPercentage(CharmRarity rarity, EquipmentStat stat, float rollStrength)
    {
        (float min, float max) bounds = GetStatBounds(stat, rarity);
        float min = bounds.min;
        float max = bounds.max;

        if (Math.Abs(max - min) < 0.00001f) return 100f;

        float quality = (rollStrength - min) / (max - min) * 100f;
        return (float)Math.Round(Math.Clamp(quality, 0f, 100f), 0);
    }

    public static float GetRollQuality(EquipmentStat stat, CharmRarity rarity, float currentStrength)
    {
        (float min, float max) = GetStatBounds(stat, rarity);
        if (Math.Abs(max - min) < float.Epsilon) return 1f; // Prevent division by zero

        return MathHelper.Clamp((currentStrength - min) / (max - min), 0f, 1f);
    }

    public static float CalculateStrengthFromQuality(EquipmentStat stat, CharmRarity rarity, float quality)
    {
        (float min, float max) = GetStatBounds(stat, rarity);
        return min + quality * (max - min);
    }

    /// <summary>
    ///     Rarity accounted for
    /// </summary>
    public static (float min, float max) GetStatBounds(EquipmentStat stat, CharmRarity rarity)
    {
        if (stat == null || !StatDefinitions.TryGetValue(stat, out var def)) return (0f, 0f);

        float mul = statRarityMultipliers.GetValueOrDefault(rarity, 1f);

        return (def.min * mul, def.max * mul);
    }

    public static bool IsCharmRareEnoughForStat(EquipmentStat stat, CharmRarity rarity)
    {
        if (stat == null || !StatDefinitions.TryGetValue(stat, out var def)) return false;

        return rarity >= def.minRarity;
    }

    public static CharmRarity GetStatRarity(EquipmentStat stat)
    {
        if (stat == null || !StatDefinitions.TryGetValue(stat, out var def)) return CharmRarity.Common;

        return def.minRarity;
    }

    #endregion

    #region 6. String Utilities

    public static string SplitCamelCase(CharmName charmRarity)
    {
        return SplitCamelCase(charmRarity.ToString());
    }

    private static string SplitCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        StringBuilder result = new();

        foreach (char c in input)
        {
            if (char.IsUpper(c) && result.Length > 0) result.Append(' ');

            result.Append(c);
        }

        return result.ToString();
    }

    #endregion
}