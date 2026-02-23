using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Detours;
using EquipmentEvolved.Assets.ModPlayers;
using EquipmentEvolved.Assets.ModPrefixes.Pickaxe;
using EquipmentEvolved.Assets.ModPrefixes.Tool;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Misc;

public class ToolPrefixManipulation : ModSystem
{
    public override void PostSetupContent()
    {
        TileBreaking.OnPlayerKilledTile += OnTileKilled;
    }

    private void OnTileKilled(Player player, int x, int y, Item item, int type, bool wall)
    {
        if (player.whoAmI != Main.myPlayer) return;
        if (item == null || item.IsAir || item.prefix < PrefixID.Count) return;

        ModPrefix itemPrefix = PrefixLoader.GetPrefix(item.prefix);
        if (itemPrefix is not ISpecializedPrefix) return;
        
        int prefixType = itemPrefix.Type;

        if (prefixType == ModContent.PrefixType<PrefixClearing>())
            ClearingPrefix(x, y, wall);
        else if (prefixType == ModContent.PrefixType<PrefixVeinMiner>())
            VeinMinerPrefix(x, y, item, type);
        //else if (prefixType == ModContent.PrefixType<PrefixFortune>())
            //FortunePrefix(player);
        else if (prefixType == ModContent.PrefixType<PrefixRevealing>())
            RevealingPrefix(player);
    }

    private static void RevealingPrefix(Player player)
    {
        if (Main.rand.NextFloat() <= PrefixBalance.REVEALING_CHANCE)
        {
            player.AddBuff(BuffID.Spelunker, 20);
            player.GetModPlayer<ToolPlayer>().SetRevealing();
        }
    }

    private static void VeinMinerPrefix(int x, int y, Item pickaxe, int type)
    {
        bool isOre = TileID.Sets.Ore[type];
        bool isGem = type is >= TileID.Sapphire and <= TileID.Diamond or TileID.AmberStoneBlock;

        if (!isOre && !isGem) return;
        
        List<Point> connectedTiles = UtilMethods.GetConnectedTiles(x, y, pickaxe.pick, type);
        foreach (Point t in connectedTiles)
        {
            KillTileAndSync(new Point(t.X, t.Y), loseItem: false, wall: false);
        }
    }


    private static void ClearingPrefix(int x, int y, bool wall)
    {
        List<Point> area = GetSquareCoordinates(new Point(x, y), 1); // Radius 1 = 3x3 area

        foreach (Point pos in area)
        {
            // Skip the center tile (it's already dead)
            if (pos.X == x && pos.Y == y) continue;

            if (wall)
            {
                if (Main.tile[pos.X, pos.Y].WallType > WallID.None)
                {
                    bool loseItem = Main.rand.NextFloat() < PrefixBalance.CLEARING_CHANCE_TO_LOSE_MINED_BLOCK;
                    KillTileAndSync(pos, loseItem, true);
                }
            }
            else
            {
                Tile tile = Main.tile[pos.X, pos.Y];
                if (tile.HasTile && IsSafeToDestroy(pos.X, pos.Y, tile.TileType))
                {
                    bool loseItem = Main.rand.NextFloat() < PrefixBalance.CLEARING_CHANCE_TO_LOSE_MINED_BLOCK;
                    KillTileAndSync(pos, loseItem, false);
                }
            }
        }
    }
    
    private static bool IsSafeToDestroy(int x, int y, int type)
    {
        // 1. DANGEROUS TILES (Chests, Dressers, Altars)
        if (TileID.Sets.BasicChest[type] || TileID.Sets.BasicDresser[type] || type == TileID.DemonAltar) 
            return false;

        // 2. TREES (Pickaxes shouldn't break trees)
        if (TileID.Sets.IsATreeTrunk[type]) 
            return false;

        // 3. TILES SUPPORTING CHESTS (Don't break the floor under a chest)
        // Check the tile directly above. If it's a chest, this tile is vital.
        Tile tileAbove = Main.tile[x, y - 1];
        if (tileAbove != null && tileAbove.HasTile)
        {
            if (TileID.Sets.BasicChest[tileAbove.TileType] || TileID.Sets.BasicDresser[tileAbove.TileType])
                return false;
        }

        return true;
    }

    private static List<Point> GetSquareCoordinates(Point center, int radius)
    {
        List<Point> coordinates = new();
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                coordinates.Add(new Point(center.X + x, center.Y + y));
            }
        }
        return coordinates;
    }

    /// <summary>
    /// Helper to safely break tiles/walls and sync changes. 
    /// Now uses standard WorldGen methods instead of custom replacements.
    /// </summary>
    public static void KillTileAndSync(Point pos, bool loseItem, bool wall)
    {
        // 1. Apply Locally
        if (wall)
        {
            WorldGen.KillWall(pos.X, pos.Y, fail: false);
        }
        else
        {
            WorldGen.KillTile(pos.X, pos.Y, fail: false, effectOnly: false, noItem: loseItem);
        }
        
        // 2. Sync to Server (Custom Packet)
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            ModPacket packet = ModContent.GetInstance<EquipmentEvolved>().GetPacket();
            packet.Write((byte)MessageType.SilentTileKill);
            packet.Write(pos.X);
            packet.Write(pos.Y);
            packet.Write(wall);
            packet.Write(loseItem);
            packet.Send();
        }
    }
}