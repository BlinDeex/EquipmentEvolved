using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Detours;

public class TileBreaking : ModSystem
{
    public delegate void PlayerKilledTile(Player player, int x, int y, Item item, int tileType, bool wall = false);
    public static event PlayerKilledTile OnPlayerKilledTile;

    public override void Load()
    {
        On_Player.PickTile += OnPickTile;
        On_Player.ItemCheck_UseMiningTools_TryHittingWall += OnTryHitWall;
    }

    private void OnPickTile(On_Player.orig_PickTile orig, Player self, int x, int y, int pickPower)
    {
        bool tileExisted = Main.tile[x, y].HasTile;
        int tileType = Main.tile[x, y].TileType;
        orig(self, x, y, pickPower);
        
        if (tileExisted && !Main.tile[x, y].HasTile)
        {
            OnPlayerKilledTile?.Invoke(self, x, y, self.HeldItem, tileType, wall: false);
        }
    }

    private void OnTryHitWall(On_Player.orig_ItemCheck_UseMiningTools_TryHittingWall orig, Player self, Item sItem, int wX, int wY)
    {
        bool wallExisted = Main.tile[wX, wY].WallType > WallID.None;
        int wallType = Main.tile[wX, wY].WallType;
        orig(self, sItem, wX, wY);
        
        if (wallExisted && Main.tile[wX, wY].WallType == WallID.None)
        {
            OnPlayerKilledTile?.Invoke(self, wX, wY, sItem, wallType, wall: true);
        }
    }
}