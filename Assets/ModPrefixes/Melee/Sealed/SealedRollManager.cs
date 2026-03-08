using System.Collections.Generic;
using System.Linq;
using EquipmentEvolved.Assets.CharmsModule.Data;
using EquipmentEvolved.Assets.Core;
using Terraria;
using Terraria.Utilities;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.Sealed;

public class SealedRollDefinition
{
    public PlayerStat Stat { get; init; }
    public float MinValue { get; init; }
    public float MaxValue { get; init; }
    public double Weight { get; init; }
}

public static class SealedRollManager
{
    private static readonly Dictionary<float, int> rollCountProbabilities = new()
    {
        { 0.05f, 1 },
        { 0.1f, 2 },
        { 0.3f, 3 },
        { 0.5f, 4 },
        { 0.7f, 5 },
        { 0.8f, 6 },
        { 0.95f, 7 },
        { 0.998f, 10 }
    };

    private static readonly Dictionary<float, float> rollStrengthProbabilities = new()
    {
        { 0.05f, 1f },
        { 0.1f, 1.2f },
        { 0.3f, 1.4f },
        { 0.5f, 1.8f },
        { 0.7f, 2.4f },
        { 0.8f, 3f },
        { 0.95f, 5f },
        { 0.998f, 20f }
    };

    private static readonly List<SealedRollDefinition> rollPool =
    [
        // Percentages: 0.20f means 20%, 0.30f means 30%
        new() { Stat = PlayerStat.CharmLuck, MinValue = 0.20f, MaxValue = 1.00f, Weight = 1.0f },
        new() { Stat = PlayerStat.CritDamage, MinValue = 0.01f, MaxValue = 0.40f, Weight = 0.7f },
        new() { Stat = PlayerStat.Damage, MinValue = 0.01f, MaxValue = 0.30f, Weight = 0.5f },

        // Flat Stats: 2f means 2 frames, 4f means 4 health/sec
        new() { Stat = PlayerStat.Regen, MinValue = 0.5f, MaxValue = 4f, Weight = 0.8f },
        new() { Stat = PlayerStat.LifeSteal, MinValue = 0.5f, MaxValue = 2f, Weight = 0.2f },
        new() { Stat = PlayerStat.Iframes, MinValue = 2f, MaxValue = 16f, Weight = 0.2f }
    ];

    private static int GetRollsCount()
    {
        float dice = Main.rand.NextFloat(0f, 1f);

        foreach (KeyValuePair<float, int> kvp in rollCountProbabilities.OrderBy(x => x.Key))
        {
            if (dice <= kvp.Key) return kvp.Value;
        }

        return rollCountProbabilities.Values.Max();
    }

    private static CharmRoll GenerateRoll(WeightedRandom<SealedRollDefinition> pool)
    {
        SealedRollDefinition selected = pool.Get();
        float baseStrength = Main.rand.NextFloat(selected.MinValue, selected.MaxValue);

        float dice = Main.rand.NextFloat(0f, 1f);
        float multiplier = 1f;

        foreach (KeyValuePair<float, float> kvp in rollStrengthProbabilities.OrderBy(x => x.Key))
        {
            if (dice <= kvp.Key)
            {
                multiplier = kvp.Value;
                break;
            }
        }

        return new CharmRoll(selected.Stat, baseStrength * multiplier);
    }

    public static List<CharmRoll> GenerateRolls()
    {
        List<CharmRoll> rolls = [];
        int numRolls = GetRollsCount();

        WeightedRandom<SealedRollDefinition> pool = new(Main.rand);
        foreach (SealedRollDefinition def in rollPool)
        {
            pool.Add(def, def.Weight);
        }

        for (int i = 0; i < numRolls; i++)
        {
            rolls.Add(GenerateRoll(pool));
        }

        return rolls;
    }
}