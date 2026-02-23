using System;
using System.Collections.Generic;
using System.Text;
using EquipmentEvolved.Assets.NPCs;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.CharmsModule;

public static class CharmBalance
{
    public static int RollTriesForBosses = 10;
    public static int MinStatRolls = 1;
    public static int MaxStatRolls = 3;

    public static int LegendaryPity = 30;
    public static int MythicalPity = 100;

    public static List<int> ExcludedNPCSFromCharmDrops = new()
    {
        ModContent.NPCType<ChallengerOrb>()
    };
    
    public static Dictionary<float, CharmType> CharmTypeChance = new()
    {
        { 0.33f, CharmType.Circle },
        { 0.33f + 0.33f, CharmType.Square },
        { 1f, CharmType.Triangle }
    };

    public static List<(float chance, CharmRarity rarity)> CharmRarityDropTable = new()
    {
        (0.01f, CharmRarity.Mythical),  // 0.00 to 0.01 (1% chance)
        (0.03f, CharmRarity.Legendary), // 0.01 to 0.03 (2% chance)
        (0.07f, CharmRarity.Epic),      // 0.03 to 0.07 (4% chance)
        (0.17f, CharmRarity.Rare),      // 0.07 to 0.17 (10% chance)
        (0.37f, CharmRarity.Common)     // 0.17 to 0.37 (20% chance)
    };
    
    public static Dictionary<CharmType, Texture2D> CharmTextures = new()
    {
        { CharmType.NotInitialized , ModContent.Request<Texture2D>($"{nameof(EquipmentEvolved)}/Assets/CharmsModule/Textures/Charm_NoTex", AssetRequestMode.ImmediateLoad).Value},
        { CharmType.Circle , ModContent.Request<Texture2D>($"{nameof(EquipmentEvolved)}/Assets/CharmsModule/Textures/Charm_Circle", AssetRequestMode.ImmediateLoad).Value},
        { CharmType.Square , ModContent.Request<Texture2D>($"{nameof(EquipmentEvolved)}/Assets/CharmsModule/Textures/Charm_Square", AssetRequestMode.ImmediateLoad).Value},
        { CharmType.Triangle , ModContent.Request<Texture2D>($"{nameof(EquipmentEvolved)}/Assets/CharmsModule/Textures/Charm_Triangle", AssetRequestMode.ImmediateLoad).Value}
    };

    private static Dictionary<CharmRarity, Func<Color>> CharmColors = new()
    {
        { CharmRarity.NotInitialized , () => new Color(255,255,255)},
        { CharmRarity.Common, () => new Color(255, 255, 255) },
        { CharmRarity.Rare, () => new Color(100, 100, 255) },
        { CharmRarity.Epic, () => new Color(255, 0, 255) },
        { CharmRarity.Legendary, () => new Color(255, 255, 0) },
        { CharmRarity.Mythical, () => new Color(255, 100, 100) },
        { CharmRarity.Exalted, () => 
            {
                float pulse = (MathF.Sin(Main.GlobalTimeWrappedHourly * 3f) + 1f) / 2f;
                return Color.Lerp(Main.DiscoColor, Color.White, 0.2f + 0.6f * pulse); 
            }
        },
    };

    public static Color GetCharmColor(CharmRarity rarity)
    {
        if (CharmColors.TryGetValue(rarity, out Func<Color> color)) return color.Invoke();
        
        UtilMethods.BroadcastOrNewText($"{nameof(GetCharmColor)}: {nameof(CharmColors)} didnt contain rarity of {rarity.ToString()}!", Color.Red);
        return Color.Black;
    }

    private static Dictionary<CharmStat, (float min, float max)> StatBounds = new()
    {
        { CharmStat.NotInitialized, (0, 0) },
        { CharmStat.Damage, (0.01f, 0.02f) },
        { CharmStat.MeleeDamage, (0.01f, 0.02f) },
        { CharmStat.RangedDamage, (0.01f, 0.02f) },
        { CharmStat.MagicDamage, (0.01f, 0.02f) },
        { CharmStat.SummonDamage, (0.01f, 0.02f) },
        { CharmStat.MoveSpeed, (0.005f, 0.01f) },
        { CharmStat.WingTime, (3f, 7f) },
        { CharmStat.UseSpeed, (0.01f, 0.03f) },
        { CharmStat.LifeSteal, (0.1f, 0.2f) },
        { CharmStat.CharmLuck, (0.01f, 0.2f) },
        { CharmStat.CritDamage, (0.01f, 0.02f) },
        { CharmStat.ManaUsage, (-0.02f, -0.04f) },
        { CharmStat.HealingMul, (0.01f, 0.03f) },
        { CharmStat.Regen, (0.05f, 0.1f) },
        { CharmStat.PickSpeed, (0.02f, 0.05f) },
        { CharmStat.MaxHealthMul, (0.01f, 0.02f) },
        { CharmStat.Crit, (0.03f, 0.05f) },
        { CharmStat.Iframes, (0.5f, 1.2f)},
        { CharmStat.DamageReduction, (0.03f, 0.07f)},
        { CharmStat.TrueDamageMul, (0.06f, 0.13f)}
    };

    private static Dictionary<CharmStat, CharmRarity> StatRarities = new()
    {
        { CharmStat.NotInitialized, CharmRarity.NotInitialized },
        { CharmStat.MeleeDamage, CharmRarity.Common },
        { CharmStat.RangedDamage, CharmRarity.Common },
        { CharmStat.MagicDamage, CharmRarity.Common },
        { CharmStat.SummonDamage, CharmRarity.Common },
        { CharmStat.MoveSpeed, CharmRarity.Common },
        { CharmStat.CharmLuck, CharmRarity.Common },
        { CharmStat.PickSpeed, CharmRarity.Common },
        { CharmStat.Crit, CharmRarity.Common },
        { CharmStat.Damage, CharmRarity.Rare },
        { CharmStat.WingTime, CharmRarity.Rare },
        { CharmStat.UseSpeed, CharmRarity.Rare },
        { CharmStat.ManaUsage, CharmRarity.Rare },
        { CharmStat.Regen, CharmRarity.Rare },
        { CharmStat.LifeSteal, CharmRarity.Epic },
        { CharmStat.CritDamage, CharmRarity.Epic },
        { CharmStat.HealingMul, CharmRarity.Epic },
        { CharmStat.MaxHealthMul, CharmRarity.Epic },
        { CharmStat.Iframes, CharmRarity.Legendary},
        { CharmStat.DamageReduction, CharmRarity.Legendary},
        { CharmStat.TrueDamageMul, CharmRarity.Mythical}
    };

    private static readonly Array charmStatValues = Enum.GetValues(typeof(CharmStat));
    
    public static CharmStat GetRandomStat()
    {
        while (true)
        {
            CharmStat stat = (CharmStat)charmStatValues.GetValue(Main.rand.Next(charmStatValues.Length))!;
            if (stat != CharmStat.NotInitialized) return stat;
        }
    }

    public static bool StatDisplayValue(CharmStat stat)
    {
        bool multiply = stat switch
        {
            CharmStat.LifeSteal => false,
            CharmStat.WingTime => false,
            CharmStat.Iframes => false,
            _ => true
        };
        return multiply;
    }
    
    /// <summary>
    /// Calculates how perfect a charm stat roll is, returning a value from 0.00 to 100.00
    /// </summary>
    public static float GetRollQualityPercentage(CharmRarity rarity, CharmStat stat,  float rollStrength)
    {
        // Get the allowed bounds for this specific stat and rarity
        var bounds = GetStatBounds(stat, rarity);
        float min = bounds.min;
        float max = bounds.max;

        // Prevent division by zero in case a stat has a fixed value (min == max)
        if (Math.Abs(max - min) < 0.00001f)
        {
            return 100f;
        }

        // Calculate the percentage
        // This math automatically handles negative bounds (like ManaUsage) gracefully
        float quality = (rollStrength - min) / (max - min) * 100f;

        // Clamp the result between 0 and 100 to prevent weird UI numbers due to float rounding errors,
        // and round it to 2 decimal places (e.g., 80.22)
        return (float)Math.Round(Math.Clamp(quality, 0f, 100f), 0);
    }
    
    /// <summary>
    /// Multiplies stat bound by this value
    /// </summary>
    public static readonly Dictionary<CharmRarity, float> StatRarityMultipliers = new()
    {
        { CharmRarity.Common, 1f },
        { CharmRarity.Rare, 2f },
        { CharmRarity.Epic, 4f },
        { CharmRarity.Legendary, 8f },
        { CharmRarity.Mythical, 16f },
        { CharmRarity.Exalted, 20f }
    };

    public static float GetRarityMultiplier(CharmRarity rarity)
    {
        if (!StatRarityMultipliers.TryGetValue(rarity, out float statRarity))
        {
            throw new ArgumentException($"Stat multiplier not defined for {rarity}");
        }

        return statRarity;
    }
    
    public static Color GetStatColor(CharmStat stat)
    {
        CharmRarity rarity = GetStatRarity(stat);
        return GetCharmColor(rarity);
    }

    public static bool IsCharmRareEnoughForStat(CharmStat stat, CharmRarity rarity)
    {
        if (!StatRarities.TryGetValue(stat, out CharmRarity statRarity))
        {
            throw new ArgumentException($"Stat rarity not defined for {stat}");
        }
        
        return rarity >= statRarity;
    }

    public static string SplitCamelCase(CharmName charmRarity) => SplitCamelCase(charmRarity.ToString());
    
    public static string SplitCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }
        
        StringBuilder result = new System.Text.StringBuilder();

        foreach (char c in input)
        {
            if (char.IsUpper(c) && result.Length > 0)
            {
                result.Append(' ');
            }
            result.Append(c);
        }

        return result.ToString();
    }
    
    

    public static CharmRarity GetStatRarity(CharmStat stat)
    {
        if (!StatRarities.TryGetValue(stat, out CharmRarity statRarity))
        {
            throw new ArgumentException($"Stat rarity not defined for {stat}");
        }

        return statRarity;
    }
    
    /// <summary>
    /// Rarity accounted for
    /// </summary>
    public static (float min, float max) GetStatBounds(CharmStat stat, CharmRarity rarity)
    {
        if (!StatBounds.TryGetValue(stat, out (float min, float max) bounds))
        {
            throw new ArgumentException($"Stat bounds not defined for {stat}");
        }

        if (!StatRarityMultipliers.TryGetValue(rarity, out float mul))
        {
            throw new ArgumentException($"Rarity multiplier is not defined for {rarity}");
        }
        
        return (bounds.min * mul, bounds.max * mul);
    }
}