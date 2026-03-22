using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.CrossMod.Examples;

/// <summary>
/// This file demonstrates how an EXTERNAL mod (e.g., Calamity, Thorium, or a custom addon) 
/// can interact with Equipment Evolved's Stat System without needing a hard reference to the .dll!
/// </summary>
public class ExampleCrossModIntegration : ModSystem
{
    // We store the generated string ID so we can easily reference our custom stat later
    public static string CustomStealthStatID;

    public override void PostSetupContent()
    {
        // 1. Check if Equipment Evolved is loaded
        if (!ModLoader.TryGetMod("EquipmentEvolved", out Mod eeMod)) return;

        // 2. Define how the stat should look on a tooltip
        // Example: If value is 15.5f, it returns "+15 Rogue Stealth"
        Func<float, string> stealthFormatter = (val) => $"+{(int)val} Rogue Stealth";

        // 3. Register the stat dynamically!
        // Arguments: "RegisterDynamicStat", ModName, StatName, TooltipFormatter, StackingMode (0 = Additive)
        object result = eeMod.Call("RegisterDynamicStat", Mod.Name, "RogueStealth", stealthFormatter, 0);
        
        if (result is string statId && statId.StartsWith("DynamicEquipmentStat/"))
        {
            CustomStealthStatID = statId;

            // 4. INJECT INTO RNG POOLS
            // Now that the stat exists, we can add it to Equipment Evolved's random drop pools using our string ID!
            
            // Add to Charms Pool: (ID, Rarity [1=Rare], Min Roll, Max Roll)
            eeMod.Call("RegisterCharmStat", CustomStealthStatID, 1, 5f, 15f);

            // Add to Sealed Weapons: (ID, Min Roll, Max Roll, Weight)
            eeMod.Call("RegisterSealedStat", CustomStealthStatID, 5f, 20f, 0.5);

            // Add to Chaotic Weapons: (ID, Min Roll, Max Roll)
            eeMod.Call("RegisterChaoticStat", CustomStealthStatID, 5f, 30f);
            
            Mod.Logger.Info("Successfully registered Rogue Stealth into Equipment Evolved!");
        }
    }
}

/// <summary>
/// Demonstrates how to manually give a CUSTOM stat to a player from an item.
/// </summary>
public class ExampleAddonAccessory : ModItem
{
    // Tell tModLoader to just use the Shackle sprite so it doesn't crash looking for a custom PNG!
    public override string Texture => "Terraria/Images/Item_" + ItemID.Shackle;

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (ModLoader.TryGetMod("EquipmentEvolved", out Mod eeMod) && ExampleCrossModIntegration.CustomStealthStatID != null)
        {
            // Manually add +10 Rogue Stealth to the player's Equipment Evolved Ledger
            // Arguments: "AddStatToPlayer", Player, StatID, Value, SourceFlag (1 = Generic)
            eeMod.Call("AddStatToPlayer", player, ExampleCrossModIntegration.CustomStealthStatID, 10f, 1);
        }
    }
}

/// <summary>
/// Demonstrates how to manually give an ALREADY EXISTING Equipment Evolved stat to a player from an item.
/// </summary>
public class ExampleExistingStatAccessory : ModItem
{
    public override string Texture => "Terraria/Images/Item_" + ItemID.TitanGlove;

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (ModLoader.TryGetMod("EquipmentEvolved", out Mod eeMod))
        {
            // By passing the class name of the stat (e.g., "MeleeDamageStat"), we can utilize built-in stats!
            // This adds +15% Melee Damage using EE's ledger system.
            eeMod.Call("AddStatToPlayer", player, "MeleeDamageStat", 0.15f, 1);

            // We can also tap into the custom utility stats, like granting +20% Wing Horizontal Acceleration
            eeMod.Call("AddStatToPlayer", player, "WingHorizontalAccStat", 0.20f, 1);
        }
    }
}

/// <summary>
/// Demonstrates how to read the total value of stats (both custom and existing) to apply your own logic.
/// </summary>
public class ExampleAddonPlayer : ModPlayer
{
    public float TotalRogueStealth;
    public float TotalLifesteal;

    public override void ResetEffects()
    {
        TotalRogueStealth = 0f;
        TotalLifesteal = 0f;
    }

    public override void PostUpdateEquips()
    {
        if (ModLoader.TryGetMod("EquipmentEvolved", out Mod eeMod))
        {
            // --- 1. Reading a Custom Stat ---
            if (ExampleCrossModIntegration.CustomStealthStatID != null)
            {
                object stealthResult = eeMod.Call("GetTotalStat", Player, ExampleCrossModIntegration.CustomStealthStatID);
                if (stealthResult is float totalStealth)
                {
                    TotalRogueStealth = totalStealth;
                    // Example usage: Player.GetDamage<RogueDamageClass>() += (TotalRogueStealth / 100f);
                }
            }

            // --- 2. Reading an Existing Stat ---
            // Pass the class name of the stat you want to read. Let's check how much Lifesteal the player has!
            object lifestealResult = eeMod.Call("GetTotalStat", Player, "LifestealStat");
            if (lifestealResult is float totalLifesteal)
            {
                TotalLifesteal = totalLifesteal;
                
                // Example usage: You could use this value to grant a custom buff if they have over 5% lifesteal
                // if (TotalLifesteal > 0.05f) Player.AddBuff(ModContent.BuffType<VampiricAuraBuff>(), 2);
            }
        }
    }
}