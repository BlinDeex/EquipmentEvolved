using EquipmentEvolved.Assets.Misc;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Utilities;

public static class MiningUtils
{
    public static void KillTileAndSync(Point pos, bool loseItem, bool wall)
    {
        if (wall)
            WorldGen.KillWall(pos.X, pos.Y);
        else
            WorldGen.KillTile(pos.X, pos.Y, false, false, loseItem);

        if (Main.netMode != NetmodeID.MultiplayerClient) return;
        
        ModPacket packet = ModContent.GetInstance<EquipmentEvolved>().GetPacket();
        packet.Write((byte)MessageType.SilentTileKill);
        packet.Write(pos.X);
        packet.Write(pos.Y);
        packet.Write(wall);
        packet.Write(loseItem);
        packet.Send();
    }
}