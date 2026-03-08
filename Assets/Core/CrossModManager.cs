using System;
using System.Collections.Generic;
using System.Reflection;
using EquipmentEvolved.Assets.Balance;

namespace EquipmentEvolved.Assets.Core;

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
}