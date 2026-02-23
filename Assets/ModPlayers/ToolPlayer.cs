using System.IO;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using Terraria.ModLoader;
// Ensure MessageType is here

namespace EquipmentEvolved.Assets.ModPlayers;

public class ToolPlayer : ModPlayer
{
    public int AxeFortune { get; set; } // Changed to public set for internal use
    public int RevealingTicks { get; private set; }

    public void SetRevealing()
    {
        RevealingTicks = PrefixBalance.REVEALING_TICKS;
    }

    // Call this when the Axe is SWUNG (not just when tile breaks)
    public void ActivateFortune()
    {
        AxeFortune = 30; // Active for 10 ticks (approx 0.16s), enough to cover the tile break
    }

    public override void PostUpdateEquips()
    {
        if (AxeFortune > 0) AxeFortune--;
        if (RevealingTicks > 0) RevealingTicks--;
    }

    // --- SYNCING (Critical for Multiplayer) ---
    public override void CopyClientState(ModPlayer targetCopy)
    {
        ToolPlayer clone = (ToolPlayer)targetCopy;
        clone.AxeFortune = AxeFortune;
    }

    public override void SendClientChanges(ModPlayer clientPlayer)
    {
        ToolPlayer clone = (ToolPlayer)clientPlayer;
        if (clone.AxeFortune != AxeFortune) // Only send if changed
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)MessageType.SyncToolPlayer); // Add this to your Enums!
            packet.Write((byte)Player.whoAmI);
            packet.Write(AxeFortune);
            packet.Send();
        }
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        ModPacket packet = Mod.GetPacket();
        packet.Write((byte)MessageType.SyncToolPlayer);
        packet.Write((byte)Player.whoAmI);
        packet.Write(AxeFortune);
        packet.Send(toWho, fromWho);
    }
    
    public void ReceiveSync(BinaryReader reader)
    {
        AxeFortune = reader.ReadInt32();
    }
}