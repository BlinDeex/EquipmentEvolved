using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Globals.Items;
using EquipmentEvolved.Assets.ModPlayers;
using EquipmentEvolved.Assets.ModPrefixes.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static EquipmentEvolved.Assets.Misc.SharedLocalization;

namespace EquipmentEvolved.Assets.Balance;

public static class ChaoticRollPool
{
    private static readonly List<RollStats> rolls = new();
    private static RollStats currentRollStats;

    private enum RollRarity
    {
        Common,
        Rare,
        Epic,
        Legendary,
        Negative,
        Debug
    }

    private static float maximumRollValue;

    private static int ticksElapsedSinceLastRoll = 9999;

    private static int ROLL_LENGTH = PrefixBalance.CHAOTIC_ROLL_LENGTH;

    private static StatPlayer statPlayer;

    private static RollStats RollChaotic()
    {
        float dice = Main.rand.NextFloat(0, maximumRollValue);
        int chaoticRollIndex = 0;
        float noIdeaHowToNameThisDescriptively =
            rolls[chaoticRollIndex]
                .Chance; // each chaotic roll chance is added to this and chaoticRollIndex is incremented,
        // when this becomes bigger than dice, apply chaotic roll with index of chaoticRollIndex value

        for (int i = 1; i < rolls.Count; i++)
        {
            if (dice < noIdeaHowToNameThisDescriptively) break;
            noIdeaHowToNameThisDescriptively += rolls[chaoticRollIndex].Chance;
            chaoticRollIndex++;
        }

        return rolls[chaoticRollIndex];
    }

    public static void Load()
    {
        rolls.Add(new RollStats(RollRarity.Common)
        {
            DamageMultiplier = 1.2f,
            ManaUsageMultiplier = 0.9f
        });
        rolls.Add(new RollStats(RollRarity.Common)
        {
            ManaUsageMultiplier = 0.75f,
            Crit = 5
        });
        rolls.Add(new RollStats(RollRarity.Common)
        {
            UseSpeedMultiplier = 0.9f,
            CritDamageMultiplier = 1.1f
        });
        rolls.Add(new RollStats(RollRarity.Common)
        {
            DamageMultiplier = 1.1f,
            LifeSteal = 1
        });
        rolls.Add(new RollStats(RollRarity.Common)
        {
            MaxHealthDamagePercentage = 0.05f,
            CritDamageMultiplier = 1.15f
        });
        rolls.Add(new RollStats(RollRarity.Common)
        {
            DamageMultiplier = 1.15f,
            UseSpeedMultiplier = 0.95f
        });
        rolls.Add(new RollStats(RollRarity.Common)
        {
            Crit = 10,
            ManaUsageMultiplier = 0.85f
        });
        rolls.Add(new RollStats(RollRarity.Common)
        {
            DamageMultiplier = 1.25f,
            UseSpeedMultiplier = 0.85f
        });
        rolls.Add(new RollStats(RollRarity.Common)
        {
            DamageMultiplier = 1.1f,
            CritDamageMultiplier = 1.2f
        });
        rolls.Add(new RollStats(RollRarity.Common)
        {
            DamageMultiplier = 1.1f,
            AverageCoinDropValue = 10f
        });

        // Rare rolls (3 stats)
        rolls.Add(new RollStats(RollRarity.Rare)
        {
            DamageMultiplier = 1.5f,
            ManaUsageMultiplier = 0.75f,
            Crit = 15
        });
        rolls.Add(new RollStats(RollRarity.Rare)
        {
            Crit = 15,
            UseSpeedMultiplier = 0.85f,
            LifeSteal = 2
        });
        rolls.Add(new RollStats(RollRarity.Rare)
        {
            LifeSteal = 3,
            DamageMultiplier = 1.3f,
            CritDamageMultiplier = 1.15f
        });
        rolls.Add(new RollStats(RollRarity.Rare)
        {
            MaxHealthDamagePercentage = 0.1f,
            CritDamageMultiplier = 1.15f,
            ManaUsageMultiplier = 0.7f
        });
        rolls.Add(new RollStats(RollRarity.Rare)
        {
            DamageMultiplier = 1.7f,
            ManaUsageMultiplier = 0.65f,
            Crit = 20
        });
        rolls.Add(new RollStats(RollRarity.Rare)
        {
            Crit = 20,
            DamageMultiplier = 1.4f,
            LifeSteal = 2
        });
        rolls.Add(new RollStats(RollRarity.Rare)
        {
            DamageMultiplier = 1.35f,
            LifeSteal = 2,
            AverageCoinDropValue = 40f
        });
        rolls.Add(new RollStats(RollRarity.Rare)
        {
            UseSpeedMultiplier = 0.8f,
            AverageCoinDropValue = 50,
            CritDamageMultiplier = 1.25f
        });
        rolls.Add(new RollStats(RollRarity.Rare)
        {
            CritDamageMultiplier = 1.25f,
            ManaUsageMultiplier = 0.7f,
            DamageMultiplier = 1.6f
        });
        rolls.Add(new RollStats(RollRarity.Rare)
        {
            DamageMultiplier = 1.6f,
            Crit = 25,
            LifeSteal = 3
        });

        // Epic rolls (4-5 stats)
        rolls.Add(new RollStats(RollRarity.Epic)
        {
            DamageMultiplier = 2f,
            ManaUsageMultiplier = 0.5f,
            Crit = 40,
            UseSpeedMultiplier = 0.7f
        });
        rolls.Add(new RollStats(RollRarity.Epic)
        {
            LifeSteal = 5,
            MaxHealthDamagePercentage = 0.15f,
            DamageMultiplier = 1.8f,
            Crit = 50
        });
        rolls.Add(new RollStats(RollRarity.Epic)
        {
            CritDamageMultiplier = 1.35f,
            AverageCoinDropValue = 140,
            DamageMultiplier = 2.2f,
            ManaUsageMultiplier = 0.4f
        });
        rolls.Add(new RollStats(RollRarity.Epic)
        {
            UseSpeedMultiplier = 0.7f,
            Crit = 50,
            LifeSteal = 4,
            MaxHealthDamagePercentage = 0.2f
        });
        rolls.Add(new RollStats(RollRarity.Epic)
        {
            DamageMultiplier = 2.2f,
            ManaUsageMultiplier = 0.4f,
            Crit = 60,
            AverageCoinDropValue = 160f
        });
        rolls.Add(new RollStats(RollRarity.Epic)
        {
            Crit = 60,
            LifeSteal = 4,
            DamageMultiplier = 1.9f,
            UseSpeedMultiplier = 0.65f,
            MaxHealthDamagePercentage = 0.2f
        });
        rolls.Add(new RollStats(RollRarity.Epic)
        {
            MaxHealthDamagePercentage = 0.2f,
            CritDamageMultiplier = 1.4f,
            Crit = 70,
            AverageCoinDropValue = 100f,
            ManaUsageMultiplier = 0.3f
        });
        rolls.Add(new RollStats(RollRarity.Epic)
        {
            DamageMultiplier = 1.9f,
            UseSpeedMultiplier = 0.65f,
            Crit = 70,
            CritDamageMultiplier = 1.45f,
            AverageCoinDropValue = 80f
        });
        rolls.Add(new RollStats(RollRarity.Epic)
        {
            Crit = 70,
            AverageCoinDropValue = 60f,
            ManaUsageMultiplier = 0.3f,
            CritDamageMultiplier = 1.45f,
            DamageMultiplier = 2.5f
        });
        rolls.Add(new RollStats(RollRarity.Epic)
        {
            DamageMultiplier = 2.5f,
            ManaUsageMultiplier = 0.3f,
            CritDamageMultiplier = 1.45f,
            Crit = 70
        });

        // Legendary rolls (6+ stats)
        rolls.Add(new RollStats(RollRarity.Legendary)
        {
            DamageMultiplier = 6f,
            ManaUsageMultiplier = 0.1f,
            Crit = 100,
            LifeSteal = 5,
            CritDamageMultiplier = 1.4f,
            AverageCoinDropValue = 2f,
            MaxHealthDamagePercentage = 0.02f
        });
        rolls.Add(new RollStats(RollRarity.Legendary)
        {
            DamageMultiplier = 7f,
            UseSpeedMultiplier = 0.5f,
            Crit = 120,
            LifeSteal = 6,
            AverageCoinDropValue = 500f,
            MaxHealthDamagePercentage = 0.3f
        });
        rolls.Add(new RollStats(RollRarity.Legendary)
        {
            MaxHealthDamagePercentage = 0.3f,
            CritDamageMultiplier = 1.6f,
            DamageMultiplier = 5f,
            ManaUsageMultiplier = 0.05f,
            Crit = 150,
            AverageCoinDropValue = 550f
        });
        rolls.Add(new RollStats(RollRarity.Legendary)
        {
            Crit = 150,
            ManaUsageMultiplier = 0.05f,
            AverageCoinDropValue = 750f,
            DamageMultiplier = 8f,
            LifeSteal = 7,
            CritDamageMultiplier = 1.7f
        });
        rolls.Add(new RollStats(RollRarity.Legendary)
        {
            DamageMultiplier = 8f,
            LifeSteal = 7,
            CritDamageMultiplier = 1.7f,
            UseSpeedMultiplier = 0.4f,
            MaxHealthDamagePercentage = 0.4f,
            Crit = 200
        });
        rolls.Add(new RollStats(RollRarity.Legendary)
        {
            DamageMultiplier = 9f,
            Crit = 200,
            ManaUsageMultiplier = 0.02f,
            LifeSteal = 8,
            CritDamageMultiplier = 1.8f,
            UseSpeedMultiplier = 0.3f
        });
        rolls.Add(new RollStats(RollRarity.Legendary)
        {
            MaxHealthDamagePercentage = 0.4f,
            CritDamageMultiplier = 1.8f,
            LifeSteal = 8,
            DamageMultiplier = 10f,
            ManaUsageMultiplier = 0.02f,
            Crit = 250,
            AverageCoinDropValue = 240
        });

        // Negative rolls (2 to 6 stats)
        rolls.Add(new RollStats(RollRarity.Negative)
        {
            DamageMultiplier = 0.5f,
            ManaUsageMultiplier = 1.5f
        });
        rolls.Add(new RollStats(RollRarity.Negative)
        {
            Crit = -50,
            UseSpeedMultiplier = 1.5f
        });
        rolls.Add(new RollStats(RollRarity.Negative)
        {
            LifeSteal = -5,
            DamageMultiplier = 0.7f
        });
        rolls.Add(new RollStats(RollRarity.Negative)
        {
            MaxHealthDamagePercentage = -0.1f,
            CritDamageMultiplier = 0.8f
        });
        rolls.Add(new RollStats(RollRarity.Negative)
        {
            DamageMultiplier = 0.8f,
            ManaUsageMultiplier = 1.25f,
            Crit = -50
        });
        rolls.Add(new RollStats(RollRarity.Negative)
        {
            Crit = -50,
            UseSpeedMultiplier = 1.25f,
            LifeSteal = -3
        });
        rolls.Add(new RollStats(RollRarity.Negative)
        {
            LifeSteal = -4,
            DamageMultiplier = 0.6f,
            CritDamageMultiplier = 0.75f,
            ManaUsageMultiplier = 1.35f
        });
        rolls.Add(new RollStats(RollRarity.Negative)
        {
            MaxHealthDamagePercentage = -0.15f,
            CritDamageMultiplier = 0.7f,
            DamageMultiplier = 0.5f,
            ManaUsageMultiplier = 1.5f,
            Crit = -100
        });
        rolls.Add(new RollStats(RollRarity.Negative)
        {
            DamageMultiplier = 0.5f,
            ManaUsageMultiplier = 1.5f,
            Crit = -100,
            LifeSteal = -4,
            UseSpeedMultiplier = 1.4f
        });
        rolls.Add(new RollStats(RollRarity.Negative)
        {
            DamageMultiplier = 0.4f,
            LifeSteal = -5,
            CritDamageMultiplier = 0.6f,
            MaxHealthDamagePercentage = -0.2f, //TODO: implement negative dmg
            ManaUsageMultiplier = 1.5f,
            Crit = -150
        });

        foreach (RollStats roll in rolls) maximumRollValue += roll.Chance;
    }

    public static void Tick(Player player)
    {
        if (player.whoAmI != Main.myPlayer) return;
        if (player.HeldItem.prefix != ModContent.PrefixType<PrefixChaotic>()) return;
        ticksElapsedSinceLastRoll++;

        statPlayer = player.GetModPlayer<StatPlayer>();

        if (ticksElapsedSinceLastRoll > ROLL_LENGTH)
        {
            currentRollStats = RollChaotic();
            ticksElapsedSinceLastRoll = 0;
            ApplyCurrentRoll();
        }
    }


    private class RollStats(RollRarity rollRarity)
    {
        public RollRarity RollRarity => rollRarity;
        public float Chance => GetRollChance(rollRarity);

        public float DamageMultiplier;
        public float ManaUsageMultiplier;
        public float UseSpeedMultiplier;
        public int Crit;
        public int LifeSteal;
        public float CritDamageMultiplier;
        public float MaxHealthDamagePercentage;
        public float AverageCoinDropValue;
    }

    private static void ApplyCurrentRoll()
    {
        RollStats cs = currentRollStats;
        List<TooltipLine> tooltipLines = new();

        if (cs.DamageMultiplier != 0)
        {
            float statDisplayValue = MathF.Round((cs.DamageMultiplier - 1) * 100f, 2);
            statPlayer.DamageMul += cs.DamageMultiplier - 1;
            Color tooltipColor = cs.DamageMultiplier > 1
                ? GetToolTipColor(cs.RollRarity)
                : PrefixBalance.CHAOTIC_NEGATIVE_ROLL_COLOR;
            string tooltipText = cs.DamageMultiplier > 1
                ? GetSharedLocalizedText(XDamageAdded).Format(statDisplayValue)
                : GetSharedLocalizedText(XDamageDecreased).Format(statDisplayValue);

            TooltipLine newLine = new(EquipmentEvolved.Instance, nameof(newLine), tooltipText)
            {
                OverrideColor = tooltipColor
            };
            tooltipLines.Add(newLine);
        }

        if (cs.ManaUsageMultiplier != 0)
        {
            float statDisplayValue = MathF.Abs(MathF.Round((1 - cs.ManaUsageMultiplier) * 100f, 2));
            statPlayer.ManaUsageMul += 1 - cs.ManaUsageMultiplier;
            Color tooltipColor = cs.ManaUsageMultiplier < 1
                ? GetToolTipColor(cs.RollRarity)
                : PrefixBalance.CHAOTIC_NEGATIVE_ROLL_COLOR;
            string tooltipText = cs.ManaUsageMultiplier < 1
                ? GetSharedLocalizedText(XDecreasedManaUsage).Format(statDisplayValue)
                : GetSharedLocalizedText(XIncreasedManaUsage).Format(statDisplayValue);

            TooltipLine newLine = new(EquipmentEvolved.Instance, nameof(newLine), tooltipText)
            {
                OverrideColor = tooltipColor
            };
            tooltipLines.Add(newLine);
        }

        if (cs.UseSpeedMultiplier != 0)
        {
            float statDisplayValue = MathF.Abs(MathF.Round((1 - cs.UseSpeedMultiplier) * 100f, 2));
            statPlayer.UseTimeMul -= 1 - cs.UseSpeedMultiplier;
            Color tooltipColor = cs.UseSpeedMultiplier < 1
                ? GetToolTipColor(cs.RollRarity)
                : PrefixBalance.CHAOTIC_NEGATIVE_ROLL_COLOR;
            string tooltipText = cs.UseSpeedMultiplier < 1
                ? GetSharedLocalizedText(XUseTimeReduced).Format(statDisplayValue)
                : GetSharedLocalizedText(XUseTimeIncreased).Format(statDisplayValue);

            TooltipLine newLine = new(EquipmentEvolved.Instance, nameof(newLine), tooltipText)
            {
                OverrideColor = tooltipColor
            };
            tooltipLines.Add(newLine);
        }

        if (cs.Crit != 0)
        {
            float statDisplayValue = cs.Crit;
            statPlayer.Crit += cs.Crit;
            Color tooltipColor = cs.Crit > 0 ? GetToolTipColor(cs.RollRarity) : PrefixBalance.CHAOTIC_NEGATIVE_ROLL_COLOR;
            string tooltipText = cs.Crit > 0
                ? GetSharedLocalizedText(XCritAdded).Format(statDisplayValue)
                : GetSharedLocalizedText(XCritDecreased).Format(statDisplayValue);

            TooltipLine newLine = new(EquipmentEvolved.Instance, nameof(newLine), tooltipText)
            {
                OverrideColor = tooltipColor
            };
            tooltipLines.Add(newLine);
        }

        if (cs.LifeSteal != 0)
        {
            float statDisplayValue = cs.LifeSteal;
            statPlayer.Lifesteal += cs.LifeSteal;
            Color tooltipColor = cs.LifeSteal > 0
                ? GetToolTipColor(cs.RollRarity)
                : PrefixBalance.CHAOTIC_NEGATIVE_ROLL_COLOR;
            string tooltipText = cs.LifeSteal > 0
                ? GetSharedLocalizedText(XIncreasedLifesteal).Format(statDisplayValue)
                : GetSharedLocalizedText(XDecreasedLifesteal).Format(statDisplayValue);

            TooltipLine newLine = new(EquipmentEvolved.Instance, nameof(newLine), tooltipText)
            {
                OverrideColor = tooltipColor
            };
            tooltipLines.Add(newLine);
        }

        if (cs.CritDamageMultiplier != 0)
        {
            float statDisplayValue = MathF.Abs(MathF.Round((1 - cs.CritDamageMultiplier) * 100f, 2));
            statPlayer.CritDamageMul += cs.CritDamageMultiplier - 1;
            Color tooltipColor = cs.CritDamageMultiplier > 1
                ? GetToolTipColor(cs.RollRarity)
                : PrefixBalance.CHAOTIC_NEGATIVE_ROLL_COLOR;
            string tooltipText = cs.CritDamageMultiplier > 1
                ? GetSharedLocalizedText(XCritDamageIncreased).Format(statDisplayValue)
                : GetSharedLocalizedText(XCritDamageDecreased).Format(statDisplayValue);

            TooltipLine newLine = new(EquipmentEvolved.Instance, nameof(newLine), tooltipText)
            {
                OverrideColor = tooltipColor
            };
            tooltipLines.Add(newLine);
        }

        if (cs.MaxHealthDamagePercentage != 0)
        {
            float statDisplayValue = MathF.Abs(MathF.Round(cs.MaxHealthDamagePercentage, 3));
            statPlayer.TrueDamageMul += cs.MaxHealthDamagePercentage;
            Color tooltipColor = cs.MaxHealthDamagePercentage > 0
                ? GetToolTipColor(cs.RollRarity)
                : PrefixBalance.CHAOTIC_NEGATIVE_ROLL_COLOR;
            string tooltipText = cs.MaxHealthDamagePercentage > 0
                ? GetSharedLocalizedText(XPositiveMaxHealthDamage).Format(statDisplayValue)
                : GetSharedLocalizedText(XNegativeMaxHealthDamage).Format(statDisplayValue);

            TooltipLine newLine = new(EquipmentEvolved.Instance, nameof(newLine), tooltipText)
            {
                OverrideColor = tooltipColor
            };
            tooltipLines.Add(newLine);
        }

        if (cs.AverageCoinDropValue != 0)
        {
            float statDisplayValue = MathF.Abs(MathF.Round(cs.AverageCoinDropValue * 100, 2));
            statPlayer.CoinDropValue += cs.AverageCoinDropValue;
            Color tooltipColor = cs.AverageCoinDropValue > 0
                ? GetToolTipColor(cs.RollRarity)
                : PrefixBalance.CHAOTIC_NEGATIVE_ROLL_COLOR;
            string tooltipText = cs.AverageCoinDropValue > 0
                ? GetSharedLocalizedText(XIncreasedCoinDropValue).Format(statDisplayValue)
                : GetSharedLocalizedText(XDecreasedCoinDropValue).Format(statDisplayValue);

            TooltipLine newLine = new(EquipmentEvolved.Instance, nameof(newLine), tooltipText)
            {
                OverrideColor = tooltipColor
            };
            tooltipLines.Add(newLine);
        }

        GlobalMagicPrefix.CurrentChaoticTooltipLines = tooltipLines;
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
            RollRarity.Debug => PrefixBalance.CHAOTIC_DEBUG_ROLL_COLOR,
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
            RollRarity.Debug => PrefixBalance.CHAOTIC_DEBUG_GUARANTEED_ROLL_CHANCE,
            _ => throw new ArgumentOutOfRangeException(nameof(rollRarity), rollRarity, null)
        };
    }
}