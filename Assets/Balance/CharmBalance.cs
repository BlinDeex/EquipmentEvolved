using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using EquipmentEvolved.Assets.CharmsModule.Data;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.ModPrefixes.Ranged.Challenger;
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

    private static readonly Dictionary<PlayerStat, (CharmRarity minRarity, float min, float max)> StatDefinitions = new()
    {
        { PlayerStat.NotInitialized, (CharmRarity.NotInitialized, 0, 0) },

        // --- Offensive Stats ---
        { PlayerStat.Damage, (CharmRarity.Common, 0.01f, 0.02f) },
        { PlayerStat.MeleeDamage, (CharmRarity.Common, 0.015f, 0.03f) },
        { PlayerStat.RangedDamage, (CharmRarity.Common, 0.015f, 0.03f) },
        { PlayerStat.MagicDamage, (CharmRarity.Common, 0.015f, 0.03f) },
        { PlayerStat.SummonDamage, (CharmRarity.Common, 0.015f, 0.03f) },
        { PlayerStat.Crit, (CharmRarity.Common, 1f, 2f) },
        { PlayerStat.CritDamage, (CharmRarity.Epic, 0.02f, 0.05f) },
        { PlayerStat.TrueDamageMul, (CharmRarity.Legendary, 0.02f, 0.04f) },

        // --- Utility Stats ---
        { PlayerStat.UseSpeed, (CharmRarity.Rare, 0.01f, 0.02f) },
        { PlayerStat.HealthCapMul, (CharmRarity.Rare, -0.02f, -0.03f) },
        { PlayerStat.PickSpeed, (CharmRarity.Common, 0.02f, 0.05f) },
        { PlayerStat.MoveSpeed, (CharmRarity.Common, 0.01f, 0.02f) },
        { PlayerStat.WingTime, (CharmRarity.Rare, 5f, 15f) },
        { PlayerStat.ManaUsage, (CharmRarity.Rare, -0.01f, -0.03f) },
        { PlayerStat.CharmLuck, (CharmRarity.Common, 0.01f, 0.05f) },

        // --- Survivability (Highly Regulated) ---
        { PlayerStat.MaxHealthMul, (CharmRarity.Epic, 0.01f, 0.02f) },
        { PlayerStat.Regen, (CharmRarity.Rare, 0.25f, 0.75f) },
        { PlayerStat.HealingMul, (CharmRarity.Epic, 0.01f, 0.03f) },
        { PlayerStat.LifeSteal, (CharmRarity.Legendary, 0.008f, 0.016f) },
        { PlayerStat.DamageLifesteal, (CharmRarity.Epic, 0.01f, 0.03f) },
        { PlayerStat.DamageReduction, (CharmRarity.Mythical, 0.01f, 0.02f) },
        { PlayerStat.Iframes, (CharmRarity.Legendary, 1.0f, 3.0f) } 
    };

    public static readonly PlayerStat[] ValidCharmStats = StatDefinitions.Keys.Where(k => k != PlayerStat.NotInitialized).ToArray();

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

    public static Color GetStatColor(PlayerStat stat)
    {
        CharmRarity rarity = GetStatRarity(stat);
        return GetCharmColor(rarity);
    }

    #endregion

    #region 5. Mathematics & Roll Logic

    public static PlayerStat GetRandomStat()
    {
        return ValidCharmStats[Main.rand.Next(ValidCharmStats.Length)];
    }

    /// <summary>
    ///     Calculates how perfect a charm stat roll is, returning a value from 0.00 to 100.00
    /// </summary>
    public static float GetRollQualityPercentage(CharmRarity rarity, PlayerStat stat, float rollStrength)
    {
        (float min, float max) bounds = GetStatBounds(stat, rarity);
        float min = bounds.min;
        float max = bounds.max;

        if (Math.Abs(max - min) < 0.00001f) return 100f;

        float quality = (rollStrength - min) / (max - min) * 100f;
        return (float)Math.Round(Math.Clamp(quality, 0f, 100f), 0);
    }

    public static float GetRollQuality(PlayerStat stat, CharmRarity rarity, float currentStrength)
    {
        (float min, float max) = GetStatBounds(stat, rarity);
        if (Math.Abs(max - min) < float.Epsilon) return 1f; // Prevent division by zero

        return MathHelper.Clamp((currentStrength - min) / (max - min), 0f, 1f);
    }

    public static float CalculateStrengthFromQuality(PlayerStat stat, CharmRarity rarity, float quality)
    {
        (float min, float max) = GetStatBounds(stat, rarity);
        return min + quality * (max - min);
    }

    /// <summary>
    ///     Rarity accounted for
    /// </summary>
    public static (float min, float max) GetStatBounds(PlayerStat stat, CharmRarity rarity)
    {
        // FAILSAFE: Prevents crashes if you load a world with a broken charm from testing!
        if (!StatDefinitions.TryGetValue(stat, out (CharmRarity minRarity, float min, float max) def)) return (0f, 0f);

        float mul = statRarityMultipliers.GetValueOrDefault(rarity, 1f);

        return (def.min * mul, def.max * mul);
    }

    public static bool IsCharmRareEnoughForStat(PlayerStat stat, CharmRarity rarity)
    {
        if (!StatDefinitions.TryGetValue(stat, out (CharmRarity minRarity, float min, float max) def)) return false;

        return rarity >= def.minRarity;
    }

    public static CharmRarity GetStatRarity(PlayerStat stat)
    {
        if (!StatDefinitions.TryGetValue(stat, out (CharmRarity minRarity, float min, float max) def)) return CharmRarity.Common;

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