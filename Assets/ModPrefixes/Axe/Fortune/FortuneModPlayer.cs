using System.IO;
using EquipmentEvolved.Assets.Misc;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Axe.Fortune;

public class FortuneModPlayer : ModPlayer
{
    public int AxeFortune { get; set; }

    public void ActivateFortune()
    {
        AxeFortune = 30;
    }

    public override void PostUpdateEquips()
    {
        if (AxeFortune > 0) AxeFortune--;
    }

    public override void CopyClientState(ModPlayer targetCopy)
    {
        FortuneModPlayer clone = (FortuneModPlayer)targetCopy;
        clone.AxeFortune = AxeFortune;
    }

    public override void SendClientChanges(ModPlayer clientPlayer)
    {
        FortuneModPlayer clone = (FortuneModPlayer)clientPlayer;
        if (clone.AxeFortune == AxeFortune) return;

        ModPacket packet = Mod.GetPacket();
        packet.Write((byte)MessageType.SyncFortuneModPlayer);
        packet.Write((byte)Player.whoAmI);
        packet.Write(AxeFortune);
        packet.Send();
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        ModPacket packet = Mod.GetPacket();
        packet.Write((byte)MessageType.SyncFortuneModPlayer);
        packet.Write((byte)Player.whoAmI);
        packet.Write(AxeFortune);
        packet.Send(toWho, fromWho);
    }

    public void ReceiveSync(BinaryReader reader)
    {
        AxeFortune = reader.ReadInt32();
    }
}