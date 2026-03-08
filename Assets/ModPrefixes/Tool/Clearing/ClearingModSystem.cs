using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Patches;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Tool.Clearing;

public class ClearingModSystem : ModSystem
{
    public override void PostSetupContent()
    {
        TileBreaking.OnPlayerKilledTile += OnTileKilled;
    }

    private void OnTileKilled(Player player, int x, int y, Item item, int type, bool wall)
    {
        if (player.whoAmI != Main.myPlayer) return;

        if (item == null || item.IsAir || item.prefix < PrefixID.Count) return;

        if (!item.HasPrefix(ModContent.PrefixType<PrefixClearing>())) return;

        List<Point> area = GetSquareCoordinates(new Point(x, y), 1);

        foreach (Point pos in area)
        {
            if (pos.X == x && pos.Y == y) continue;

            if (wall)
            {
                if (Main.tile[pos.X, pos.Y].WallType > WallID.None)
                {
                    bool loseItem = Main.rand.NextFloat() < PrefixBalance.CLEARING_CHANCE_TO_LOSE_MINED_BLOCK;
                    MiningUtils.KillTileAndSync(pos, loseItem, true);
                }
            }
            else
            {
                Tile tile = Main.tile[pos.X, pos.Y];
                if (tile.HasTile && IsSafeToDestroy(pos.X, pos.Y, tile.TileType))
                {
                    bool loseItem = Main.rand.NextFloat() < PrefixBalance.CLEARING_CHANCE_TO_LOSE_MINED_BLOCK;
                    MiningUtils.KillTileAndSync(pos, loseItem, false);
                }
            }
        }
    }

    private static bool IsSafeToDestroy(int x, int y, int type)
    {
        if (TileID.Sets.BasicChest[type] || TileID.Sets.BasicDresser[type] || type == TileID.DemonAltar) return false;

        if (TileID.Sets.IsATreeTrunk[type]) return false;

        Tile tileAbove = Main.tile[x, y - 1];
        if (tileAbove != null && tileAbove.HasTile)
            if (TileID.Sets.BasicChest[tileAbove.TileType] || TileID.Sets.BasicDresser[tileAbove.TileType])
                return false;

        return true;
    }

    private static List<Point> GetSquareCoordinates(Point center, int radius)
    {
        List<Point> coordinates = [];
        for (int x = -radius; x <= radius; x++)
        for (int y = -radius; y <= radius; y++)
        {
            coordinates.Add(new Point(center.X + x, center.Y + y));
        }

        return coordinates;
    }
}