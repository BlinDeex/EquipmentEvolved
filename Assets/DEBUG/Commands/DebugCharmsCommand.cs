using System;
using System.Collections.Generic;
using System.Linq;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.CharmsModule.Items;
using EquipmentEvolved.Assets.CharmsModule.Data; 
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.DEBUG.Commands;

public class DebugCharmsCommand : ModCommand
{
    public override CommandType Type => CommandType.Chat;

    public override string Command => "debugcharms";

    public override string Usage => "/debugcharms";

    public override string Description => "Spawns a Circle charm for every single EquipmentStat with a strength of 10.";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        Player player = caller.Player;
        
        var allStats = ModContent.GetContent<EquipmentStat>().ToList();
        
        int spawnedCount = 0;
        
        foreach (EquipmentStat stat in allStats)
        {
            Item item = new Item();
            item.SetDefaults(ModContent.ItemType<Charm>());
            
            if (item.ModItem is Charm charm)
            {
                // 1. Properly initialize the core data so the UI wakes up
                charm.CharmType = CharmType.Circle; 
                charm.CharmRarity = CharmRarity.Legendary; // Sets the color and tooltip tier
                charm.UniqueID = Guid.NewGuid(); // Prevents stacking/UI errors
                charm.CharmNameID = 31; // 31 is the starting ID for Legendary names based on your manager
                
                // 2. Inject the stat using the exact constructor from your manager
                charm.Stats = new List<CharmRoll>
                {
                    new CharmRoll(stat, 10f)
                };

                player.QuickSpawnItem(player.GetSource_Misc("DebugCharmsCommand"), item);
                spawnedCount++;
            }
        }

        Main.NewText($"Successfully generated {spawnedCount} debug charms!", Color.LimeGreen);
    }
}