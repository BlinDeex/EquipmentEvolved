using System;
using System.Reflection;
using EquipmentEvolved.Assets.CharmsModule.Items;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.Utilities;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.CharmsModule.CrossMod;

/// <summary>
/// Bypasses Magic Storage fast-withdrawal optimization. 
/// MS ignores custom ModItem data (like our Guid) during extraction equivalence checks. 
/// Intercepts the withdrawal, finds exact Charm via Guid, and spoofs its prefix 
/// to force Magic Storage to extract the correct instance.
/// </summary>
public class MagicStorageFix : ModSystem
{
    private Hook magicStorageWithdrawHook;

    public override void Load()
    {
        if (!ModLoader.TryGetMod("MagicStorage", out Mod magicStorage)) return;
        
        Type storageHeartType = magicStorage.Code.GetType("MagicStorage.Components.TEStorageHeart");
        if (storageHeartType == null)
        {
            UtilMethods.LogMessage("Failed to find TEStorageHeart type in Magic Storage, charm withdrawal fix will not be applied.", LogType.Error);
            return;
        }

        MethodInfo withdrawMethod = storageHeartType.GetMethod(
            "TryWithdraw", 
            BindingFlags.Public | BindingFlags.Instance, 
            null, [typeof(Item), typeof(bool), typeof(bool)],
            null
        );

        if (withdrawMethod != null)
        {
            magicStorageWithdrawHook = new Hook(withdrawMethod, OnMagicStorageWithdraw);
            magicStorageWithdrawHook.Apply();
        }
        else
        {
            UtilMethods.LogMessage("Failed to find TryWithdraw method in Magic Storage Heart, charm withdrawal fix will not be applied.", LogType.Error);
        }
    }

    public override void Unload()
    {
        magicStorageWithdrawHook?.Undo();
        magicStorageWithdrawHook = null;
    }
    
    private delegate Item WithdrawDelegate(object self, Item targetItem, bool toInventory, bool keepOneIfFavorite);

    private Item OnMagicStorageWithdraw(WithdrawDelegate orig, object self, Item targetItem, bool toInventory, bool keepOneIfFavorite)
    {
        if (targetItem.ModItem is not Charm targetCharm) return orig(self, targetItem, toInventory, keepOneIfFavorite);
        
        Type heartType = self.GetType();
            
        MethodInfo getUnitsMethod = heartType.GetMethod("GetStorageUnits", BindingFlags.Public | BindingFlags.Instance);
        if (getUnitsMethod == null)
        {
            UtilMethods.LogMessage("Failed to find GetStorageUnits method in Magic Storage Heart, charms withdraw operation will work incorrectly.", LogType.Error);
            return orig(self, targetItem, toInventory, keepOneIfFavorite);
        }
            
        var units = (System.Collections.IEnumerable)getUnitsMethod.Invoke(self, null);
                
        foreach (object unit in units!)
        {
            Type unitType = unit.GetType();
            System.Collections.IEnumerable items = null;
                    
            MethodInfo getItemsMethod = unitType.GetMethod("GetItems", BindingFlags.Public | BindingFlags.Instance);
            if (getItemsMethod != null) 
            {
                items = (System.Collections.IEnumerable)getItemsMethod.Invoke(unit, null);
            }
            else 
            {
                FieldInfo itemsField = unitType.GetField("items", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (itemsField != null) items = (System.Collections.IEnumerable)itemsField.GetValue(unit);
            }

            if (items == null)
            {
                UtilMethods.LogMessage($"Failed to find items collection in Magic Storage unit ({unitType.Name}), charms withdraw operation might fail.", LogType.Error);
                continue; 
            }

            foreach (Item storedItem in items)
            {
                if (storedItem == null || storedItem.IsAir || storedItem.ModItem is not Charm storedCharm || storedCharm.UniqueID != targetCharm.UniqueID) continue;
                
                int origStoredPrefix = storedItem.prefix;
                int origTargetPrefix = targetItem.prefix;
                            
                storedItem.prefix = 254;
                targetItem.prefix = 254;
                            
                try
                {
                    Item extractedItem = orig(self, targetItem, toInventory, keepOneIfFavorite);
                                
                    if (extractedItem != null && !extractedItem.IsAir) 
                    {
                        extractedItem.prefix = origStoredPrefix;
                    }
                                
                    return extractedItem;
                }
                finally
                {
                    targetItem.prefix = origTargetPrefix;
                    storedItem.prefix = origStoredPrefix;
                }
            }
        }

        return orig(self, targetItem, toInventory, keepOneIfFavorite);
    }
}