using System;
using System.Collections.Generic;

namespace EquipmentEvolved.Assets.Core;

internal class DynamicEquipmentStat(string customName, Func<float, string> tooltipFormatter, StatStackingMode stackingMode) : EquipmentStat
{
    public override string Name => customName;
    public override StatStackingMode StackingMode => stackingMode;

    public override string FormatTooltip(float value) => tooltipFormatter(value);
}

public static class EquipmentStatLoader
{
    private static readonly List<EquipmentStat> statsByNetId = [];
    private static readonly Dictionary<string, EquipmentStat> statsByFullName = new();

    public static int StatCount => statsByNetId.Count;

    internal static void RegisterStat(EquipmentStat stat)
    {
        stat.NetID = statsByNetId.Count;
        statsByNetId.Add(stat);
        
        string modName = stat.Mod?.Name ?? "DynamicEquipmentStat";
        statsByFullName[$"{modName}/{stat.Name}"] = stat;
    }
    
    public static string AddDynamicStat(string externalModName, string statName, Func<float, string> tooltipFormatter, int stackingModeInt = 0)
    {
        string internalName = $"{externalModName}_{statName}";
        var dynamicStat = new DynamicEquipmentStat(internalName, tooltipFormatter, (StatStackingMode)stackingModeInt);
        
        RegisterStat(dynamicStat);
        
        // Return the exact dictionary key so the external mod knows how to reference it later
        return $"DynamicEquipmentStat/{internalName}";
    }

    public static EquipmentStat GetStatByNetID(int netId)
    {
        if (netId >= 0 && netId < statsByNetId.Count)
            return statsByNetId[netId];
        return null;
    }

    public static bool TryGetStatByFullName(string fullName, out EquipmentStat stat)
    {
        return statsByFullName.TryGetValue(fullName, out stat);
    }

    internal static void Unload()
    {
        statsByNetId.Clear();
        statsByFullName.Clear();
    }
}