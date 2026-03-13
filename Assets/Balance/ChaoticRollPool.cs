using System;
using System.Collections.Generic;
using System.Linq;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.ModPrefixes.Magic.Chaotic;
using EquipmentEvolved.Assets.ModPrefixes.Magic.Endless;
using EquipmentEvolved.Assets.ModPrefixes.Magic.Inverted;
using EquipmentEvolved.Assets.ModPrefixes.Magic.ManaCharged;
using EquipmentEvolved.Assets.ModPrefixes.Magic.Splintering;
using EquipmentEvolved.Assets.ModPrefixes.Magic.TripleShot;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;
using static EquipmentEvolved.Assets.Misc.LocalizationManager;

namespace EquipmentEvolved.Assets.Balance;

public static class ChaoticRollPool
{
    private static readonly List<int> validFakedPrefixes =
    [
        ModContent.PrefixType<PrefixTripleShot>(),
        ModContent.PrefixType<PrefixSplintering>(),
        ModContent.PrefixType<PrefixManaCharged>(),
        ModContent.PrefixType<PrefixInverted>(),
        ModContent.PrefixType<PrefixEndless>()
    ];

    private static readonly List<StatRange> AvailableStats =
    [
        new() { Stat = PlayerStat.Damage, MinValue = 0.02f, MaxValue = 0.05f },
        new() { Stat = PlayerStat.Crit, MinValue = 1f, MaxValue = 3f },
        new() { Stat = PlayerStat.DamageLifesteal, MinValue = 0.005f, MaxValue = 0.015f },
        new() { Stat = PlayerStat.CritDamage, MinValue = 0.02f, MaxValue = 0.06f }, 
        new() { Stat = PlayerStat.TrueDamageMul, MinValue = 0.005f, MaxValue = 0.015f },
        new() { Stat = PlayerStat.CoinDropOnHit, MinValue = 1f, MaxValue = 4f }, 

        new() { Stat = PlayerStat.ManaUsage, MinValue = 0.01f, MaxValue = 0.04f },
        new() { Stat = PlayerStat.UseSpeed, MinValue = 0.01f, MaxValue = 0.03f },

        new() { Stat = PlayerStat.MoveSpeed, MinValue = 0.02f, MaxValue = 0.04f }, 
        new() { Stat = PlayerStat.FlatDefense, MinValue = 1f, MaxValue = 3f },
        new() { Stat = PlayerStat.Regen, MinValue = 0.25f, MaxValue = 0.75f }, 
        new() { Stat = PlayerStat.Iframes, MinValue = 0.5f, MaxValue = 1.5f } 
    ];

    private static readonly Dictionary<RollRarity, (int MinStats, int MaxStats, float Intensity)> RarityConfig = new()
    {
        { RollRarity.Common, (1, 2, 1.0f) },
        { RollRarity.Rare, (2, 3, 1.6f) },
        { RollRarity.Epic, (3, 4, 2.4f) },
        { RollRarity.Legendary, (4, 5, 3.5f) },
        { RollRarity.Negative, (2, 3, -1.2f) },
    };

    private static RollStats currentRollStats;
    private static int ticksElapsedSinceLastRoll = 9999;
    private static readonly int ROLL_LENGTH = PrefixBalance.CHAOTIC_ROLL_LENGTH;
    private static StatPlayer statPlayer;

    public static int CurrentFakedPrefixId { get; private set; }

    private static RollStats RollChaotic()
    {
        // 1. Pick Rarity
        WeightedRandom<RollRarity> rarityPool = new(Main.rand);
        foreach (RollRarity rarity in Enum.GetValues(typeof(RollRarity)))
        {
            rarityPool.Add(rarity, GetRollChance(rarity));
        }

        RollRarity selectedRarity = rarityPool.Get();
        RollStats newRoll = new(selectedRarity);

        (int MinStats, int MaxStats, float Intensity) config = RarityConfig[selectedRarity];
        int statCount = Main.rand.Next(config.MinStats, config.MaxStats + 1);
        
        List<StatRange> shuffledStats = AvailableStats.OrderBy(_ => Main.rand.Next()).ToList();

        for (int i = 0; i < statCount && i < shuffledStats.Count; i++)
        {
            StatRange statDef = shuffledStats[i];
            
            float baseValue = Main.rand.NextFloat(statDef.MinValue, statDef.MaxValue);
            float finalValue = baseValue * config.Intensity;

            newRoll.Modifications.Add(new ChaoticStatMod { Stat = statDef.Stat, Value = finalValue });
        }

        CurrentFakedPrefixId = validFakedPrefixes[Main.rand.Next(validFakedPrefixes.Count)];

        return newRoll;
    }

    public static void Tick(Player player, bool isHoldingChaotic)
    {
        if (player.whoAmI != Main.myPlayer) return;

        ticksElapsedSinceLastRoll++;
        statPlayer = player.GetModPlayer<StatPlayer>();
        if (ticksElapsedSinceLastRoll > ROLL_LENGTH || currentRollStats == null)
        {
            currentRollStats = RollChaotic();
            ticksElapsedSinceLastRoll = 0;
            GenerateTooltipData();
        }

        if (isHoldingChaotic) ApplyStatsContinuously();
    }

    private static void GenerateTooltipData()
    {
        List<(string Name, string Text, Color Color)> tooltipData = [];

        if (CurrentFakedPrefixId != 0)
        {
            ModPrefix fakedPrefix = PrefixLoader.GetPrefix(CurrentFakedPrefixId);
            if (fakedPrefix != null)
            {
                string prefixName = fakedPrefix.DisplayName.Value;
                tooltipData.Add(("FakedPrefixHeader", $"[ Chaotic Shift: {prefixName} ]", Color.Cyan));
            }
        }

        Color tooltipColor = GetToolTipColor(currentRollStats.RollRarity);

        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (ChaoticStatMod mod in currentRollStats.Modifications)
        {
            float value = mod.Value;
            bool isPositiveOutcome = value > 0;

            string tooltipText = mod.Stat switch
            {
                PlayerStat.Damage => (isPositiveOutcome ? GetSharedLocalizedText(XDamageAdded) : GetSharedLocalizedText(XDamageDecreased)).Format(MathF.Round(MathF.Abs(value * 100f), 2)),
                PlayerStat.ManaUsage => (isPositiveOutcome ? GetSharedLocalizedText(XDecreasedManaUsage) : GetSharedLocalizedText(XIncreasedManaUsage)).Format(MathF.Round(MathF.Abs(value * 100f), 2)),
                PlayerStat.UseSpeed => (isPositiveOutcome ? GetSharedLocalizedText(XUseTimeReduced) : GetSharedLocalizedText(XUseTimeIncreased)).Format(MathF.Round(MathF.Abs(value * 100f), 2)),
                PlayerStat.Crit => (isPositiveOutcome ? GetSharedLocalizedText(XCritAdded) : GetSharedLocalizedText(XCritDecreased)).Format(MathF.Round(MathF.Abs(value), 0)),
                PlayerStat.LifeSteal => (isPositiveOutcome ? GetSharedLocalizedText(XIncreasedLifesteal) : GetSharedLocalizedText(XDecreasedLifesteal)).Format(MathF.Round(MathF.Abs(value), 0)),
                PlayerStat.CritDamage => (isPositiveOutcome ? GetSharedLocalizedText(XCritDamageIncreased) : GetSharedLocalizedText(XCritDamageDecreased)).Format(MathF.Round(MathF.Abs(value * 100f),
                    2)),
                PlayerStat.TrueDamageMul => (isPositiveOutcome ? GetSharedLocalizedText(XPositiveMaxHealthDamage) : GetSharedLocalizedText(XNegativeMaxHealthDamage)).Format(
                    MathF.Round(MathF.Abs(value), 3)),
                PlayerStat.CoinDropOnHit => (isPositiveOutcome ? GetSharedLocalizedText(XIncreasedCoinDropValue) : GetSharedLocalizedText(XDecreasedCoinDropValue)).Format(
                    MathF.Round(MathF.Abs(value * 100f), 2)),
                PlayerStat.MoveSpeed => (isPositiveOutcome ? GetSharedLocalizedText(XMovementSpeedIncreased) : GetSharedLocalizedText(XMovementSpeedDecreased)).Format(
                    MathF.Round(MathF.Abs(value * 100f), 2)),
                PlayerStat.FlatDefense => (isPositiveOutcome ? GetSharedLocalizedText(XDefenseIncreased) : GetSharedLocalizedText(XDefenseDecreased)).Format(MathF.Round(MathF.Abs(value), 0)),
                PlayerStat.Regen => (isPositiveOutcome ? GetSharedLocalizedText(XRegenIncreased) : GetSharedLocalizedText(XRegenDecreased)).Format(MathF.Round(MathF.Abs(value), 1)),
                PlayerStat.Iframes => (isPositiveOutcome ? GetSharedLocalizedText(XIframesIncreased) : GetSharedLocalizedText(XIframesDecreased)).Format(MathF.Round(MathF.Abs(value), 0)),
                _ => ""
            };

            tooltipData.Add(($"Chaotic_{mod.Stat}", tooltipText, tooltipColor));
        }

        ChaoticGlobalItem.CurrentTooltipData = tooltipData;
    }

    private static void ApplyStatsContinuously()
    {
        if (currentRollStats == null) return;

        foreach (ChaoticStatMod mod in currentRollStats.Modifications)
        {
            float value = mod.Value;
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (mod.Stat)
            {
                case PlayerStat.Damage: statPlayer.DamageMul += value; break;
                case PlayerStat.ManaUsage: statPlayer.ManaUsageMul -= value; break;
                case PlayerStat.UseSpeed: statPlayer.UseTimeMul -= value; break;
                case PlayerStat.Crit: statPlayer.Crit += value; break;
                case PlayerStat.LifeSteal: statPlayer.OnHitLifesteal += value; break;
                case PlayerStat.CritDamage: statPlayer.CritDamageMul += value; break;
                case PlayerStat.TrueDamageMul: statPlayer.TrueDamageMul += value; break;
                case PlayerStat.CoinDropOnHit: statPlayer.CoinDropOnHit += value; break;
                case PlayerStat.MoveSpeed: statPlayer.MovementSpeedMul += value; break;
                case PlayerStat.FlatDefense: statPlayer.FlatDefense += value; break;
                case PlayerStat.Regen: statPlayer.Regen += value; break;
                case PlayerStat.Iframes: statPlayer.Iframes += value; break;
            }
        }
    }

    private static Color GetToolTipColor(RollRarity rollRarity)
    {
        return rollRarity switch
        {
            RollRarity.Common => PrefixBalance.CHAOTIC_COMMON_ROLL_COLOR,
            RollRarity.Rare => PrefixBalance.CHAOTIC_RARE_ROLL_COLOR,
            RollRarity.Epic => PrefixBalance.CHAOTIC_EPIC_ROLL_COLOR,
            RollRarity.Legendary => PrefixBalance.CHAOTIC_LEGENDARY_ROLL_COLOR,
            RollRarity.Negative => PrefixBalance.CHAOTIC_NEGATIVE_ROLL_COLOR,
            _ => throw new ArgumentOutOfRangeException(nameof(rollRarity), rollRarity, null)
        };
    }

    private static float GetRollChance(RollRarity rollRarity)
    {
        return rollRarity switch
        {
            RollRarity.Common => PrefixBalance.CHAOTIC_COMMON_ROLL_CHANCE,
            RollRarity.Rare => PrefixBalance.CHAOTIC_RARE_ROLL_CHANCE,
            RollRarity.Epic => PrefixBalance.CHAOTIC_EPIC_ROLL_CHANCE,
            RollRarity.Legendary => PrefixBalance.CHAOTIC_LEGENDARY_ROLL_CHANCE,
            RollRarity.Negative => PrefixBalance.CHAOTIC_NEGATIVE_ROLL_CHANCE,
            _ => throw new ArgumentOutOfRangeException(nameof(rollRarity), rollRarity, null)
        };
    }

    private enum RollRarity
    {
        Common,
        Rare,
        Epic,
        Legendary,
        Negative,
    }

    private class RollStats(RollRarity rarity)
    {
        public RollRarity RollRarity { get; } = rarity;
        public List<ChaoticStatMod> Modifications { get; } = [];
    }

    private class ChaoticStatMod
    {
        public PlayerStat Stat;
        public float Value;
    }

    private class StatRange
    {
        public float MaxValue;
        public float MinValue;
        public PlayerStat Stat;
    }
}