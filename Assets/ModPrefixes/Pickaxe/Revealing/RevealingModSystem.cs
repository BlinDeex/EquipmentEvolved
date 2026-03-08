using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Patches;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Pickaxe.Revealing;

public class RevealingSystem : ModSystem
{
    public override void PostSetupContent()
    {
        TileBreaking.OnPlayerKilledTile += OnTileKilled;
    }

    private void OnTileKilled(Player player, int x, int y, Item item, int type, bool wall)
    {
        if (player.whoAmI != Main.myPlayer) return;

        if (item == null || item.IsAir || item.prefix < PrefixID.Count) return;

        if (!item.HasPrefix(ModContent.PrefixType<PrefixRevealing>())) return;

        if (!(Main.rand.NextFloat() <= PrefixBalance.REVEALING_CHANCE)) return;
        
        player.AddBuff(BuffID.Spelunker, 20);
        player.GetModPlayer<RevealingModPlayer>().SetRevealing();
    }
}