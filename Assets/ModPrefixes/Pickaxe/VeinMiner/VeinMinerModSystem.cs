using System.Collections.Generic;
using EquipmentEvolved.Assets.Patches;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Pickaxe.VeinMiner;

public class VeinMinerSystem : ModSystem
{
    public override void PostSetupContent()
    {
        TileBreaking.OnPlayerKilledTile += OnTileKilled;
    }

    private void OnTileKilled(Player player, int x, int y, Item item, int type, bool wall)
    {
        if (player.whoAmI != Main.myPlayer) return;

        if (item == null || item.IsAir || item.prefix < PrefixID.Count) return;

        if (!item.HasPrefix(ModContent.PrefixType<PrefixVeinMiner>())) return;

        bool isOre = TileID.Sets.Ore[type];
        bool isGem = type is >= TileID.Sapphire and <= TileID.Diamond or TileID.AmberStoneBlock;

        if (!isOre && !isGem) return;

        List<Point> connectedTiles = UtilMethods.GetConnectedTiles(x, y, item.pick, type);
        foreach (Point t in connectedTiles)
        {
            MiningUtils.KillTileAndSync(new Point(t.X, t.Y), false, false);
        }
    }
}