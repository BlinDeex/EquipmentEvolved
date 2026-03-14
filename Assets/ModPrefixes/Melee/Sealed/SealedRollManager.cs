using System.Collections.Generic;
using System.Linq;
using EquipmentEvolved.Assets.CharmsModule.Data;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Stats.Combat;
using EquipmentEvolved.Assets.Stats.Custom;
using EquipmentEvolved.Assets.Stats.Defense;
using EquipmentEvolved.Assets.Stats.MobilityUtility;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.Sealed;

public class SealedRollDefinition
{
    // NEW: Use EquipmentStat instead of PlayerStat
    public EquipmentStat Stat { get; init; } 
    public float MinValue { get; init; }
    public float MaxValue { get; init; }
    public double Weight { get; init; }
}

public static class SealedRollManager
{
    private static readonly Dictionary<float, int> rollCountProbabilities = new()
    {
        { 0.30f, 1 },      // 30.0% chance (0.000 to 0.300)
        { 0.55f, 2 },      // 25.0% chance (0.300 to 0.550)
        { 0.75f, 3 },      // 20.0% chance (0.550 to 0.750)
        { 0.85f, 4 },      // 10.0% chance (0.750 to 0.850)
        { 0.92f, 5 },      //  7.0% chance (0.850 to 0.920)
        { 0.97f, 6 },      //  5.0% chance (0.920 to 0.970)
        { 0.998f, 7 },     //  2.8% chance (0.970 to 0.998)
        { 1.00f, 10 }      //  0.2% chance (0.998 to 1.000)
    };

    private static readonly Dictionary<float, float> rollStrengthProbabilities = new()
    {
        { 0.30f, 1f },     // 30.0% chance
        { 0.60f, 1.2f },   // 30.0% chance
        { 0.80f, 1.4f },   // 20.0% chance
        { 0.90f, 1.8f },   // 10.0% chance
        { 0.95f, 2.4f },   //  5.0% chance
        { 0.98f, 3f },     //  3.0% chance
        { 0.998f, 5f },    //  1.8% chance
        { 1.00f, 10f }     //  0.2% chance
    };

    // NEW: Lazy-loaded property to prevent ModContent.GetInstance errors on load
    private static List<SealedRollDefinition> _rollPool;
    private static List<SealedRollDefinition> RollPool
    {
        get
        {
            if (_rollPool == null)
            {
                _rollPool =
                [
                    new() { Stat = ModContent.GetInstance<CharmLuckStat>(), MinValue = 0.05f, MaxValue = 0.15f, Weight = 1.0f },
                    new() { Stat = ModContent.GetInstance<CritDamageStat>(), MinValue = 0.01f, MaxValue = 0.05f, Weight = 0.7f },
                    new() { Stat = ModContent.GetInstance<DamageStat>(), MinValue = 0.01f, MaxValue = 0.05f, Weight = 0.5f },
                    
                    new() { Stat = ModContent.GetInstance<RegenStat>(), MinValue = 0.1f, MaxValue = 0.5f, Weight = 0.8f },
                    
                    new() { Stat = ModContent.GetInstance<LifeStealStat>(), MinValue = 0.005f, MaxValue = 0.015f, Weight = 0.2f },
                    new() { Stat = ModContent.GetInstance<IframesStat>(), MinValue = 1f, MaxValue = 3f, Weight = 0.2f }
                ];
            }
            return _rollPool;
        }
    }

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
        
        // NEW: Changed 'rollPool' to 'RollPool' to trigger the lazy load
        foreach (SealedRollDefinition def in RollPool)
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