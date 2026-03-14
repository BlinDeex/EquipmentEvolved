using System.IO;
using Terraria.ModLoader.IO;

namespace EquipmentEvolved.Assets.Core;

public class CharmRoll
{
    public EquipmentStat Stat { get; set; }
    public float Strength { get; set; }
    
    public string UnloadedStatName { get; set; } 
    public bool IsUnloaded => UnloadedStatName != null;

    public CharmRoll(EquipmentStat stat, float strength, string unloadedName = null)
    {
        Stat = stat;
        Strength = strength;
        UnloadedStatName = unloadedName;
    }
    public string GetTooltip()
    {
        return IsUnloaded ? $"[c/808080:[Missing Stat]: {UnloadedStatName}]" : Stat.FormatTooltip(Strength);
    }
    public TagCompound SaveData()
    {
        if (IsUnloaded)
        {
            return new TagCompound
            {
                ["StatFullName"] = UnloadedStatName,
                ["Strength"] = Strength
            };
        }

        string modName = Stat.Mod?.Name ?? "DynamicEquipmentStat";
        return new TagCompound
        {
            ["StatFullName"] = $"{modName}/{Stat.Name}",
            ["Strength"] = Strength
        };
    }

    public static CharmRoll LoadData(TagCompound tag)
    {
        if (tag.ContainsKey("Stat") && !tag.ContainsKey("StatFullName"))
            return null; 

        string fullName = tag.GetString("StatFullName");
        
        if (EquipmentStatLoader.TryGetStatByFullName(fullName, out var stat))
        {
            return new CharmRoll(stat, tag.GetFloat("Strength"));
        }
        
        return new CharmRoll(null, tag.GetFloat("Strength"), unloadedName: fullName);
    }
    
    public void NetSend(BinaryWriter writer)
    {
        if (IsUnloaded)
        {
            writer.Write(ushort.MaxValue); // MaxValue as a secret flag for "Unloaded"
            writer.Write(UnloadedStatName);
            writer.Write(Strength);
        }
        else
        {
            writer.Write((ushort)Stat.NetID);
            writer.Write(Strength);
        }
    }

    public static CharmRoll NetReceive(BinaryReader reader)
    {
        ushort netId = reader.ReadUInt16();
        
        if (netId == ushort.MaxValue)
        {
            string unloadedName = reader.ReadString();
            float unloadedStrength = reader.ReadSingle();
            return new CharmRoll(null, unloadedStrength, unloadedName);
        }

        float strength = reader.ReadSingle();
        var stat = EquipmentStatLoader.GetStatByNetID(netId);
        
        if (stat != null)
        {
            return new CharmRoll(stat, strength);
        }
        
        return new CharmRoll(null, strength, "Unknown_Network_Stat");
    }
}