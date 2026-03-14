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
using EquipmentEvolved.Assets.Stats.Combat;
using EquipmentEvolved.Assets.Stats.Custom;
using EquipmentEvolved.Assets.Stats.Defense;
using EquipmentEvolved.Assets.Stats.MobilityUtility;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

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

    // NEW: Lazy-loaded list to prevent ModContent.GetInstance errors during startup
    private static List<StatRange> _availableStats;
    private static List<StatRange> AvailableStats
    {
        get
        {
            if (_availableStats == null)
            {
                _availableStats =
                [
                    new() { Stat = ModContent.GetInstance<DamageStat>(), MinValue = 0.02f, MaxValue = 0.05f },
                    new() { Stat = ModContent.GetInstance<CritStat>(), MinValue = 1f, MaxValue = 3f },
                    new() { Stat = ModContent.GetInstance<DamageLifestealStat>(), MinValue = 0.005f, MaxValue = 0.015f },
                    new() { Stat = ModContent.GetInstance<CritDamageStat>(), MinValue = 0.02f, MaxValue = 0.06f }, 
                    new() { Stat = ModContent.GetInstance<TrueDamageMulStat>(), MinValue = 0.005f, MaxValue = 0.015f },
                    new() { Stat = ModContent.GetInstance<CoinDropOnHitStat>(), MinValue = 1f, MaxValue = 4f }, 

                    new() { Stat = ModContent.GetInstance<ManaUsageStat>(), MinValue = 0.01f, MaxValue = 0.04f },
                    new() { Stat = ModContent.GetInstance<UseSpeedStat>(), MinValue = 0.01f, MaxValue = 0.03f },

                    new() { Stat = ModContent.GetInstance<MoveSpeedStat>(), MinValue = 0.02f, MaxValue = 0.04f }, 
                    new() { Stat = ModContent.GetInstance<FlatDefenseStat>(), MinValue = 1f, MaxValue = 3f },
                    new() { Stat = ModContent.GetInstance<RegenStat>(), MinValue = 0.25f, MaxValue = 0.75f }, 
                    new() { Stat = ModContent.GetInstance<IframesStat>(), MinValue = 0.5f, MaxValue = 1.5f } 
                ];
            }
            return _availableStats;
        }
    }

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

        foreach (ChaoticStatMod mod in currentRollStats.Modifications)
        {
            float displayValue = mod.Value;
            
            // Replicate your old inverted math so the tooltip correctly reads it as a positive or negative buff
            if (mod.Stat is ManaUsageStat || mod.Stat is UseSpeedStat)
            {
                displayValue = -displayValue;
            }

            // NEW: The massive string of switches is gone! FormatTooltip does all the heavy lifting natively.
            string tooltipText = mod.Stat.FormatTooltip(displayValue);

            tooltipData.Add(($"Chaotic_{mod.Stat.Name}", tooltipText, tooltipColor));
        }

        ChaoticGlobalItem.CurrentTooltipData = tooltipData;
    }

    private static void ApplyStatsContinuously()
    {
        if (currentRollStats == null) return;

        foreach (ChaoticStatMod mod in currentRollStats.Modifications)
        {
            float value = mod.Value;

            // Retaining your old logic where UseSpeed and ManaUsage were subtracted
            if (mod.Stat is ManaUsageStat || mod.Stat is UseSpeedStat)
            {
                value = -value;
            }

            // NEW: Directly adding to the ledger! No switch required.
            statPlayer.AddStat(mod.Stat, value, StatSource.Charm); 
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
        // NEW: Changed from PlayerStat to EquipmentStat
        public EquipmentStat Stat;
        public float Value;
    }

    private class StatRange
    {
        // NEW: Changed from PlayerStat to EquipmentStat
        public EquipmentStat Stat;
        public float MaxValue;
        public float MinValue;
    }
}