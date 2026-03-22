using System;
using System.Collections.Generic;
using System.Reflection;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.CharmsModule.Data;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.ModPrefixes.Melee.Sealed;
using Terraria;

namespace EquipmentEvolved.Assets.CrossMod;

public static class CrossModManager
{
    public static object HandleCall(object[] args)
    {
        if (args is null || args.Length == 0 || args[0] is not string command) return "Error: First argument must be a string command name.";

        switch (command)
        {
            case "Rebalance":
                if (args.Length < 2 || args[1] is not Dictionary<string, object> dict) return $"Error: 'Rebalance' expects a {typeof(Dictionary<string, object>)} as the second argument.";

                return Rebalance(dict);

            case "RegisterDynamicStat":
                // args: (string command, string modName, string statName, Func<float, string> formatter, int stackingModeInt)
                if (args.Length < 4 || args[1] is not string extModName || args[2] is not string extStatName || args[3] is not Func<float, string> formatter)
                    return "Error: RegisterDynamicStat expects (string modName, string statName, Func<float, string> formatter, [int stackingMode = 0]).";
                
                int stackingMode = args.Length > 4 && args[4] is int sm ? sm : 0;
                return EquipmentStatLoader.AddDynamicStat(extModName, extStatName, formatter, stackingMode);

            case "AddStatToPlayer":
                // args: (string command, Player player, string statFullName, float value, int sourceInt)
                if (args.Length < 4 || args[1] is not Player pAdd || args[2] is not string statNameAdd || args[3] is not float valAdd)
                    return "Error: AddStatToPlayer expects (Player player, string statFullName, float value, [int sourceInt = 0]).";
                
                if (EquipmentStatLoader.TryGetStatByFullName(statNameAdd, out EquipmentStat statToAdd))
                {
                    int sourceInt = args.Length > 4 && args[4] is int src ? src : 0;
                    pAdd.GetModPlayer<StatPlayer>().AddStat(statToAdd, valAdd, (StatSource)sourceInt);
                    return "Success";
                }
                return "Error: Stat not found.";

            case "GetTotalStat":
                // args: (string command, Player player, string statFullName)
                if (args.Length < 3 || args[1] is not Player pGet || args[2] is not string statNameGet)
                    return "Error: GetTotalStat expects (Player player, string statFullName).";
                
                if (EquipmentStatLoader.TryGetStatByFullName(statNameGet, out EquipmentStat statToGet))
                {
                    return pGet.GetModPlayer<StatPlayer>().GetTotalStat(statToGet);
                }
                return 0f;

            // ==========================================
            // DROP POOL INJECTORS (Accepts String OR EquipmentStat)
            // ==========================================

            case "RegisterCharmStat":
                if (args.Length < 5 || args[2] is not int rarity || args[3] is not float cMin || args[4] is not float cMax)
                    return "Error: RegisterCharmStat expects (statReference, int rarity, float minBound, float maxBound).";
                
                EquipmentStat cStat = ResolveStatReference(args[1]);
                if (cStat == null) return "Error: Invalid stat reference.";

                CharmBalance.StatDefinitions[cStat] = ((CharmRarity)rarity, cMin, cMax);
                return "Success: Stat added to Charm drop pool.";

            case "RegisterSealedStat":
                if (args.Length < 5 || args[2] is not float sMin || args[3] is not float sMax || args[4] is not double weight)
                    return "Error: RegisterSealedStat expects (statReference, float min, float max, double weight).";
                
                EquipmentStat sStat = ResolveStatReference(args[1]);
                if (sStat == null) return "Error: Invalid stat reference.";

                return InjectSealedStat(sStat, sMin, sMax, weight);

            case "RegisterChaoticStat":
                if (args.Length < 4 || args[2] is not float chMin || args[3] is not float chMax)
                    return "Error: RegisterChaoticStat expects (statReference, float min, float max).";
                
                EquipmentStat chStat = ResolveStatReference(args[1]);
                if (chStat == null) return "Error: Invalid stat reference.";

                return InjectChaoticStat(chStat, chMin, chMax);

            default:
                return $"Error: Unknown command '{command}'.";
        }
    }
    
    private static EquipmentStat ResolveStatReference(object arg)
    {
        if (arg is EquipmentStat stat) return stat;
        if (arg is string statName && EquipmentStatLoader.TryGetStatByFullName(statName, out EquipmentStat dynamicStat)) return dynamicStat;
        return null;
    }

    private static string Rebalance(Dictionary<string, object> values)
    {
        List<string> errors = [];
        int successCount = 0;

        foreach ((string targetField, object targetValue) in values)
        {
            FieldInfo fieldInfo = typeof(PrefixBalance).GetField(targetField, BindingFlags.Public | BindingFlags.Static);

            if (fieldInfo == null)
            {
                errors.Add($"Field '{targetField}' not found.");
                continue;
            }

            Type fieldType = fieldInfo.FieldType;

            if (!fieldType.IsInstanceOfType(targetValue))
            {
                try
                {
                    object convertedValue = Convert.ChangeType(targetValue, fieldType);
                    fieldInfo.SetValue(null, convertedValue);
                    successCount++;
                }
                catch (Exception ex)
                {
                    errors.Add($"Failed to convert '{targetField}' to {fieldType.Name}: {ex.Message}");
                }
            }
            else
            {
                fieldInfo.SetValue(null, targetValue);
                successCount++;
            }
        }

        return errors.Count == 0 ? $"Success: {successCount} values updated." : $"Partial Success: {successCount} values updated. Errors: {string.Join(" | ", errors)}";
    }
    
    private static string InjectSealedStat(EquipmentStat stat, float min, float max, double weight)
    {
        try
        {
            Type managerType = typeof(SealedRollManager);
            
            // Force the RollPool property to generate its list if it hasn't been loaded yet
            PropertyInfo prop = managerType.GetProperty("RollPool", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            if (prop == null) return "Error: Could not find RollPool in SealedRollManager.";

            if (prop.GetValue(null) is not System.Collections.IList list) return "Error: RollPool is null.";

            Type defType = typeof(SealedRollDefinition);
            object defInstance = Activator.CreateInstance(defType);
            
            defType.GetProperty("Stat")?.SetValue(defInstance, stat);
            defType.GetProperty("MinValue")?.SetValue(defInstance, min);
            defType.GetProperty("MaxValue")?.SetValue(defInstance, max);
            defType.GetProperty("Weight")?.SetValue(defInstance, weight);

            list.Add(defInstance);
            return "Success: Stat added to Sealed weapon pool.";
        }
        catch (Exception ex)
        {
            return $"Error injecting Sealed Stat: {ex.Message}";
        }
    }

    private static string InjectChaoticStat(EquipmentStat stat, float min, float max)
    {
        try
        {
            Type poolType = typeof(ChaoticRollPool);
            
            // Force the AvailableStats property to generate its list if it hasn't been loaded yet
            PropertyInfo prop = poolType.GetProperty("AvailableStats", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            if (prop == null) return "Error: Could not find AvailableStats in ChaoticRollPool.";

            if (prop.GetValue(null) is not System.Collections.IList list) return "Error: AvailableStats is null.";

            // StatRange is a private nested class inside ChaoticRollPool, so we have to fetch it like this:
            Type defType = poolType.GetNestedType("StatRange", BindingFlags.NonPublic | BindingFlags.Public);
            if (defType == null) return "Error: Could not find StatRange class.";

            object defInstance = Activator.CreateInstance(defType);
            
            // Note: StatRange uses Fields instead of Properties!
            defType.GetField("Stat")?.SetValue(defInstance, stat);
            defType.GetField("MinValue")?.SetValue(defInstance, min);
            defType.GetField("MaxValue")?.SetValue(defInstance, max);

            list.Add(defInstance);
            return "Success: Stat added to Chaotic weapon pool.";
        }
        catch (Exception ex)
        {
            return $"Error injecting Chaotic Stat: {ex.Message}";
        }
    }
}