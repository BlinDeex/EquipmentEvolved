using System.Linq;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Patches;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Pickaxe.Geological;

public class GeologicalModSystem : ModSystem
{
    public override void PostSetupContent()
    {
        TileBreaking.OnPlayerKilledTile += OnTileKilled;
    }

    private void OnTileKilled(Player player, int x, int y, Item item, int type, bool wall)
    {
        if (player.whoAmI != Main.myPlayer) return;

        if (wall) return;

        if (item == null || item.IsAir || item.prefix < PrefixID.Count) return;

        if (!item.HasPrefix(ModContent.PrefixType<PrefixGeological>())) return;

        if (!PrefixBalance.GEOLOGICAL_VALID_TILES.Contains(type)) return;

        if (!(Main.rand.NextFloat() <= PrefixBalance.GEOLOGICAL_GEM_DROP_CHANCE)) return;

        int[] gems = PrefixBalance.GEOLOGICAL_GEM_ITEM_IDS;
        int randomGem = gems[Main.rand.Next(gems.Length)];
        int itemIndex = Item.NewItem(player.GetSource_TileInteraction(x, y), x * 16, y * 16, 16, 16, randomGem);

        if (Main.netMode == NetmodeID.MultiplayerClient) NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIndex, 1f);
    }
}