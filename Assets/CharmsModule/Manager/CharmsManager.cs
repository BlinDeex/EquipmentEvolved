using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.CharmsModule.Augmentations;
using EquipmentEvolved.Assets.CharmsModule.Items;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.CharmsModule.Manager;

public static class CharmsManager
{
    public static void Load()
    {
        Augmentations = Assembly.GetAssembly(typeof(AugmentationBase))!.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(AugmentationBase)) && !t.IsAbstract).ToArray();
    }

    private static Type[] Augmentations;

    public static List<(CharmRarity, CharmType)> RollForCharms(float luck, NPC npc = null, bool? boss = null)
    {
        List<(CharmRarity, CharmType)> rolledRarities = [];
        bool wasBoss = npc?.boss ?? (boss != null && boss.Value);
        int baseTries = wasBoss ? CharmBalance.RollTriesForBosses : 1;
        
        float calculatedTries = baseTries * luck;
        int tries = (int)calculatedTries;
        float extraTryChance = calculatedTries - tries;
        if (Main.rand.NextFloat() < extraTryChance)
        {
            tries++;
        }

        for (int i = 0; i < tries; i++)
        {
            float rarityDice = Main.rand.NextFloat(); // Rolls 0.0 to 1.0
            foreach (var drop in CharmBalance.CharmRarityDropTable)
            {
                if (rarityDice <= drop.chance)
                {
                    CharmType type = RollCharmType();
                    rolledRarities.Add((drop.rarity, type));
                    break; 
                }
            }
        }

        return rolledRarities;
    }

    public static CharmType RollCharmType()
    {
        float typeDice = Main.rand.NextFloat();
        CharmType type = CharmBalance.CharmTypeChance.First(x => x.Key >= typeDice).Value;
        return type;
    }

    private static int GetNameStartPoint(CharmRarity rollRarity)
    {
        int nameID = 1;
        if ((int)rollRarity > (int)CharmRarity.Common) nameID += 10;
        if ((int)rollRarity > (int)CharmRarity.Rare) nameID += 10;
        if ((int)rollRarity > (int)CharmRarity.Epic) nameID += 10;
        if ((int)rollRarity > (int)CharmRarity.Legendary) nameID += 10;
        if ((int)rollRarity > (int)CharmRarity.Mythical) nameID += 10;
        return nameID;
    }

    public static Vector3 GetRarityWorldColor(CharmRarity rarity)
    {
        Color rarityColor = CharmBalance.GetCharmColor(rarity);

        float multiplier = rarity switch
        {
            CharmRarity.NotInitialized => 1f,
            CharmRarity.Common => 1f,
            CharmRarity.Rare => 2f,
            CharmRarity.Epic => 3f,
            CharmRarity.Legendary => 4f,
            CharmRarity.Mythical => 5f,
            _ => throw new ArgumentOutOfRangeException(nameof(rarity), rarity, null)
        };

        return rarityColor.ToVector3() * multiplier;
    }
    
    /// <summary>
    /// Spawns charms originating from an NPC (e.g., boss drops).
    /// </summary>
    public static void SpawnCharms(List<(CharmRarity rarity, CharmType type)> rolls, int playerWhoAmI, NPC npc)
    {
        SpawnCharmsCore(rolls, playerWhoAmI, npc, npc.Center);
    }

    /// <summary>
    /// Spawns charms at a specific world coordinate (e.g., from chests or commands).
    /// </summary>
    public static void SpawnCharms(List<(CharmRarity rarity, CharmType type)> rolls, int playerWhoAmI, Vector2 spawnPos)
    {
        SpawnCharmsCore(rolls, playerWhoAmI, null, spawnPos);
    }
    
    private static void SpawnCharmsCore(List<(CharmRarity rarity, CharmType type)> rolls, int playerWhoAmI, NPC npc, Vector2 pos)
    {
        if (rolls.Count == 0) return;

        CharmPlayer charmPlayer = Main.player[playerWhoAmI].GetModPlayer<CharmPlayer>();

        foreach ((CharmRarity rollRarity, CharmType charmType) in rolls)
        {
            CreateAndSetupCharm(rollRarity, charmType, pos, playerWhoAmI, npc);
            UpdatePityAndHandleProcs(charmPlayer, rollRarity, playerWhoAmI, npc, pos);
        }
    }

    private static void CreateAndSetupCharm(CharmRarity rarity, CharmType type, Vector2 pos, int playerWhoAmI, NPC npc)
    {
        int itemType = ModContent.ItemType<Charm>();
        
        // If npc is null, GetSource_Death evaluates to null, which is fine for direct position spawns
        Item item = Main.item[Item.NewItem(npc?.GetSource_Death(), pos, itemType)];
        Charm charmItem = (Charm)item.ModItem;

        charmItem.CharmRarity = rarity;
        charmItem.CharmType = type;

        int nameID = GetNameStartPoint(rarity);
        charmItem.CharmNameID = Main.rand.Next(nameID, nameID + 10);

        if (rarity is CharmRarity.Legendary or CharmRarity.Mythical)
        {
            Color targetColor = CharmBalance.GetCharmColor(rarity);
            UtilMethods.BroadcastOrNewText($"{rarity} rarity charm has dropped for " +
                                           $"{Main.player[playerWhoAmI].name}, you have 60 seconds before anyone can pick it up!", targetColor);
            
            charmItem.SetOwner(playerWhoAmI);

            float augmentationDice = Main.rand.NextFloat();
            if (rarity == CharmRarity.Mythical || augmentationDice < PrefixBalance.LEGENDARY_AUGMENTATION_CHANCE)
            {
                charmItem.Augmentation = GetRandomAugmentation(type);
            }
        }

        RollCharmStats(charmItem, rarity);

        if (Main.netMode == NetmodeID.MultiplayerClient)
            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item.whoAmI);
    }

    private static void RollCharmStats(Charm charmItem, CharmRarity rarity)
    {
        int minRolls = CharmBalance.MinStatRolls + (int)rarity;
        int maxRolls = CharmBalance.MaxStatRolls + (int)rarity;
        int dice = Main.rand.Next(minRolls, maxRolls + 1);
        
        for (int i = 0; i < dice; i++)
        {
            CharmStat rolledStat;
            do
            {
                rolledStat = CharmBalance.GetRandomStat();
            } while (!CharmBalance.IsCharmRareEnoughForStat(rolledStat, rarity));

            (float minStatBound, float maxStatBound) = CharmBalance.GetStatBounds(rolledStat, rarity);
            float strength = Main.rand.NextFloat(minStatBound, maxStatBound);
            
            charmItem.Stats.Add(new CharmRoll(rolledStat, strength));
        }
    }

    private static void UpdatePityAndHandleProcs(CharmPlayer charmPlayer, CharmRarity droppedRarity, int playerWhoAmI, NPC npc, Vector2 pos)
    {
        if (droppedRarity is CharmRarity.Legendary or CharmRarity.Mythical)
        {
            charmPlayer.ResetPity(droppedRarity);
        }
        else
        {
            charmPlayer.IncreasePity();
        }
        if (charmPlayer.LegendaryPity >= CharmBalance.LegendaryPity)
        {
            List<(CharmRarity, CharmType)> roll = [(CharmRarity.Legendary, RollCharmType())];
            SpawnCharmsCore(roll, playerWhoAmI, npc, pos);
        }

        if (charmPlayer.MythicalPity >= CharmBalance.MythicalPity)
        {
            List<(CharmRarity, CharmType)> roll = [(CharmRarity.Mythical, RollCharmType())];
            SpawnCharmsCore(roll, playerWhoAmI, npc, pos);
        }
    }

    private static AugmentationBase GetRandomAugmentation(CharmType charmType)
    {
        if (charmType == CharmType.Circle) return null; // no augmentation for weapon charms
        Type randomType = Augmentations[Main.rand.Next(Augmentations.Length)];
        return (AugmentationBase)Activator.CreateInstance(randomType);
    }
}