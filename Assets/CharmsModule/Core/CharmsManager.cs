using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.CharmsModule.Augmentations;
using EquipmentEvolved.Assets.CharmsModule.Augmentations.WeaponAugmentations;
using EquipmentEvolved.Assets.CharmsModule.Data;
using EquipmentEvolved.Assets.CharmsModule.Items;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.Symbiotic;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace EquipmentEvolved.Assets.CharmsModule.Core;

public static class CharmsManager
{
    private static Type[] Augmentations;

    public static void Load()
    {
        List<Type> loadedAugmentations = [];
        foreach (Mod mod in ModLoader.Mods)
        {
            if (mod.Code == null) continue;
            try
            {
                IEnumerable<Type> modAugmentations = AssemblyManager.GetLoadableTypes(mod.Code) .Where(t => t.IsSubclassOf(typeof(AugmentationBase)) && !t.IsAbstract);
                loadedAugmentations.AddRange(modAugmentations);
            }
            catch (Exception e)
            {
                UtilMethods.LogMessage($"Failed to load augmentations from mod: {mod.Name}. Error: {e.Message}", LogType.Error);
            }
        }

        Augmentations = loadedAugmentations.ToArray();
    }

    public static List<(CharmRarity, CharmType)> RollForCharms(float luck, NPC npc = null, bool? boss = null)
    {
        List<(CharmRarity, CharmType)> rolledRarities = [];
        bool wasBoss = npc?.boss ?? (boss != null && boss.Value);
        int baseTries = wasBoss ? CharmBalance.RollTriesForBosses : 1;

        float calculatedTries = baseTries * luck;
        int tries = (int)calculatedTries;
        float extraTryChance = calculatedTries - tries;
        if (Main.rand.NextFloat() < extraTryChance) tries++;

        for (int i = 0; i < tries; i++)
        {
            float rarityDice = Main.rand.NextFloat(); // Rolls 0.0 to 1.0
            foreach ((float chance, CharmRarity rarity) drop in CharmBalance.CharmRarityDropTable)
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
    ///     Spawns charms originating from an NPC
    /// </summary>
    public static void SpawnCharms(List<(CharmRarity rarity, CharmType type)> rolls, int playerWhoAmI, NPC npc)
    {
        SpawnCharmsCore(rolls, playerWhoAmI, npc, npc.Center);
    }

    /// <summary>
    ///     Spawns charms at a specific world coordinate
    /// </summary>
    public static void SpawnCharms(List<(CharmRarity rarity, CharmType type)> rolls, int playerWhoAmI, Vector2 spawnPos)
    {
        SpawnCharmsCore(rolls, playerWhoAmI, null, spawnPos);
    }

    private static void SpawnCharmsCore(List<(CharmRarity rarity, CharmType type)> rolls, int playerWhoAmI, NPC npc, Vector2 pos)
    {
        if (rolls.Count == 0) return;

        Player player = Main.player[playerWhoAmI];
        CharmPlayer charmPlayer = player.GetModPlayer<CharmPlayer>();

        bool canDropExalted = ConditionsMetForExaltedCharms(player);

        foreach ((CharmRarity rollRarity, CharmType charmType) in rolls)
        {
            CharmRarity finalRarity = rollRarity;

            if (finalRarity == CharmRarity.Mythical && canDropExalted) finalRarity = CharmRarity.Exalted;

            CreateAndSetupCharm(finalRarity, charmType, pos, playerWhoAmI, npc);
            
            // NEW: Only reset pity here if a high rarity charm naturally dropped or spawned from pity. 
            // We no longer increase pity here.
            if (finalRarity is CharmRarity.Legendary or CharmRarity.Mythical or CharmRarity.Exalted)
            {
                charmPlayer.ResetPity(finalRarity);
            }
        }
    }
    
    public static void ProcessNPCKillPity(Player player, NPC npc)
    {
        CharmPlayer charmPlayer = player.GetModPlayer<CharmPlayer>();
        charmPlayer.IncreasePity();

        List<(CharmRarity rarity, CharmType type)> pityRolls = [];
        
        if (charmPlayer.LegendaryPity >= CharmBalance.LegendaryPity)
        {
            pityRolls.Add((CharmRarity.Legendary, RollCharmType()));
        }

        if (charmPlayer.MythicalPity >= CharmBalance.MythicalPity)
        {
            pityRolls.Add((CharmRarity.Mythical, RollCharmType()));
        }
        
        if (pityRolls.Count > 0)
        {
            SpawnCharmsCore(pityRolls, player.whoAmI, npc, npc.Center);
        }
    }

    private static void CreateAndSetupCharm(CharmRarity rarity, CharmType type, Vector2 pos, int playerWhoAmI, NPC npc)
    {
        int itemType = ModContent.ItemType<Charm>();
        Item item = Main.item[Item.NewItem(npc?.GetSource_Death(), pos, itemType)];
        Charm charmItem = (Charm)item.ModItem;

        charmItem.CharmRarity = rarity;
        charmItem.CharmType = type;

        int nameID = GetNameStartPoint(rarity);
        charmItem.CharmNameID = Main.rand.Next(nameID, nameID + 10);

        if (rarity is CharmRarity.Legendary or CharmRarity.Mythical or CharmRarity.Exalted)
        {
            Color targetColor = CharmBalance.GetCharmColor(rarity);
            UtilMethods.BroadcastOrNewText($"{rarity} rarity charm has dropped for " + $"{Main.player[playerWhoAmI].name}, you have 60 seconds before anyone can pick it up!", targetColor);

            charmItem.SetOwner(playerWhoAmI);

            float augmentationDice = Main.rand.NextFloat();
            if (rarity >= CharmRarity.Mythical || augmentationDice < CharmBalance.LEGENDARY_AUGMENTATION_CHANCE) charmItem.Augmentations = GetRandomAugmentations(charmItem, Main.player[playerWhoAmI]);
        }

        RollCharmStats(charmItem, rarity);

        if (Main.netMode == NetmodeID.MultiplayerClient) NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item.whoAmI);
    }

    private static void RollCharmStats(Charm charmItem, CharmRarity rarity)
    {
        (int minRolls, int maxRolls) = CharmBalance.StatRollBounds[rarity];
        int dice = Main.rand.Next(minRolls, maxRolls + 1);

        for (int i = 0; i < dice; i++)
        {
            EquipmentStat rolledStat; // Changed from PlayerStat
            do
            {
                rolledStat = CharmBalance.GetRandomStat();
            }
            while (!CharmBalance.IsCharmRareEnoughForStat(rolledStat, rarity));

            (float minStatBound, float maxStatBound) = CharmBalance.GetStatBounds(rolledStat, rarity);
            float strength = Main.rand.NextFloat(minStatBound, maxStatBound);

            charmItem.Stats.Add(new CharmRoll(rolledStat, strength));
        }
    }

    private static List<AugmentationBase> GetRandomAugmentations(Charm charm, Player player)
    {
        List<AugmentationBase> rolledAugmentations = [];

        int augCount = 0;
        if (CharmBalance.AUGMENTATION_CHANCES.TryGetValue(charm.CharmRarity, out float[] chances))
        {
            foreach (float chance in chances)
            {
                if (Main.rand.NextFloat() <= chance)
                    augCount++;
                else
                    break;
            }
        }

        if (augCount == 0) return rolledAugmentations;

        for (int i = 0; i < augCount; i++)
        {
            if (charm.CharmType == CharmType.Circle && charm.CharmRarity == CharmRarity.Exalted)
            {
                float dice = Main.rand.NextFloat();
                if (dice > CharmBalance.OMNI_AUGMENTATION_CHANCE)
                    rolledAugmentations.Add(new OmniFailedAugmentation());
                else
                    rolledAugmentations.Add(new OmniAugmentation());

                continue;
            }

            AugmentationBase rolledAug = null;
            int failsafe = 0;

            while (failsafe <= 100)
            {
                failsafe++;
                Type randomType = Augmentations[Main.rand.Next(Augmentations.Length)];
                AugmentationBase candidate = (AugmentationBase)Activator.CreateInstance(randomType);

                bool canApply = candidate!.CanApply(charm, player);
                bool alreadyHaveIt = rolledAugmentations.Any(a => a.GetType() == candidate.GetType());

                if (!canApply || alreadyHaveIt) continue;

                rolledAug = candidate;
                break;
            }

            if (rolledAug != null) rolledAugmentations.Add(rolledAug);
        }

        return rolledAugmentations;
    }

    private static bool ConditionsMetForExaltedCharms(Player player)
    {
        for (int i = 0; i < 3; i++)
        {
            Item armorPiece = player.armor[i];

            if (armorPiece == null || armorPiece.IsAir) return false;

            if (!armorPiece.HasPrefix(ModContent.PrefixType<PrefixSymbiotic>())) return false;

            if (armorPiece.TryGetGlobalItem(out CharmGlobalItem equipmentData))
            {
                // To get Exalted drops, they must be wearing Exalted charms on all 3 Symbiotic pieces
                if (equipmentData.AppliedCharmRarity != CharmRarity.Exalted) return false;
            }
            else
                return false;
        }

        return true;
    }
}