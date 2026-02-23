using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.CharmsModule.Manager;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.Utilities.Spritebatch;
using Terraria.ModLoader;

namespace EquipmentEvolved;

public class EquipmentEvolved : Mod
{
    public static EquipmentEvolved Instance { get; private set; }
    public static LogType LOG_LEVEL = LogType.Warning; // TODO: add this to config
    public override void Load()
    {
        Instance = this;
        SharedLocalization.Load();
        ChaoticRollPool.Load();
        CharmsManager.Load();
    }

    public override void Unload()
    {
        SpriteBatchSnapshotCache.Unload();
    }

    public override void PostSetupContent()
    {
        //Example();
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        Networking.Instance.HandlePacket(reader, whoAmI);
    }

    public override object Call(params object[] args)
    {
        // 1. Ensure we have at least a command string
        if (args is null || args.Length == 0 || args[0] is not string command)
        {
            return "Error: First argument must be a string command name.";
        }

        // 2. Switch based on the command
        switch (command)
        {
            case "Rebalance":
                if (args.Length < 2 || args[1] is not Dictionary<string, object> dict)
                {
                    return $"Error: 'Rebalance' expects a {typeof(Dictionary<string, object>)} as the second argument.";
                }
                return Rebalance(dict);

            default:
                return $"Error: Unknown command '{command}'.";
        }
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

    private void Example()
    {
        if (ModLoader.TryGetMod("EquipmentEvolved", out Mod EquipmentEvolved))
        {
            string result = (string)EquipmentEvolved.Call("Rebalance", exampleChanges);
            Logger.Info(result);
        }
    }

    private readonly Dictionary<string, object> exampleChanges = new()
    {
        { "ACCESSORY_REFORGING_MULTIPLIER", 2.24f },
        { "WEAPON_REFORGING_MULTIPLIER", "2.24" },
        { "EFFICIENT_MANA_SAVED", 2 }
    };
}