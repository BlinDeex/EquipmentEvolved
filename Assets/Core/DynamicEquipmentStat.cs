using System;
using System.Collections.Generic;

namespace EquipmentEvolved.Assets.Core;

// This internal class wraps stats added by OTHER mods via Mod.Call
internal class DynamicEquipmentStat : EquipmentStat
{
    private readonly string _customName;
    private readonly Func<float, string> _tooltipFormatter;

    public override string Name => _customName;

    public DynamicEquipmentStat(string customName, Func<float, string> tooltipFormatter)
    {
        _customName = customName;
        _tooltipFormatter = tooltipFormatter;
    }

    public override string FormatTooltip(float value) => _tooltipFormatter(value);
}

public static class EquipmentStatLoader
{
    private static readonly List<EquipmentStat> statsByNetId = new();
    private static readonly Dictionary<string, EquipmentStat> statsByFullName = new();

    public static int StatCount => statsByNetId.Count;

    internal static void RegisterStat(EquipmentStat stat)
    {
        stat.NetID = statsByNetId.Count;
        statsByNetId.Add(stat);
        
        string modName = stat.Mod?.Name ?? "DynamicEquipmentStat";
        statsByFullName[$"{modName}/{stat.Name}"] = stat;
    }
    
    public static void AddDynamicStat(string externalModName, string statName, Func<float, string> tooltipFormatter)
    {
        string combinedName = $"{externalModName}_{statName}";
        var dynamicStat = new DynamicEquipmentStat(combinedName, tooltipFormatter);
        
        RegisterStat(dynamicStat);
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