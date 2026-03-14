using System;
using System.Collections.Generic;
using System.IO;
using EquipmentEvolved.Assets.CharmsModule.Core;
using EquipmentEvolved.Assets.CharmsModule.Data;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.Chrono;
using EquipmentEvolved.Assets.ModPrefixes.Axe.Fortune;
using EquipmentEvolved.Assets.ModPrefixes.Magic.Inverted;
using EquipmentEvolved.Assets.ModPrefixes.Ranged.Challenger;
using EquipmentEvolved.Assets.Stats.MobilityUtility;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Misc;

public class Networking : ModSystem
{
    public static Networking Instance;

    public override void Load()
    {
        Instance = this;
    }

    public void HandlePacket(BinaryReader reader, int whoAmI)
    {
        MessageType messageType = (MessageType)reader.ReadByte();

        switch (messageType)
        {
            case MessageType.ChallengerScore:
                HandleChallengerScore(reader, whoAmI);
                break;
            case MessageType.TrueDamageText:
                HandleTrueDamageText(reader);
                break;
            case MessageType.TimeStop:
                HandleTimeStop(reader);
                break;
            case MessageType.CharmOnKilled:
                HandleCharmOnKilled(reader, whoAmI);
                break;
            case MessageType.SyncInvertedModPlayer:
                HandleSyncInvertedPlayer(reader, whoAmI);
                break;
            case MessageType.PerceptiveCritEffect:
                HandlePerceptiveCritEffect(reader, whoAmI);
                break;
            case MessageType.SilentTileKill:
                HandleSilentTileKill(reader, whoAmI);
                break;
            case MessageType.SyncFortuneModPlayer:
                HandleSyncFortuneModPlayer(reader);
                break;
            case MessageType.NegatedText:
                HandleNegatedText(reader, whoAmI);
                break;
            default:
                throw new ArgumentOutOfRangeException($"Unhandled MessageType: {messageType}");
        }
    }

    private static void HandleChallengerScore(BinaryReader reader, int whoAmI)
    {
        byte playerID = reader.ReadByte();
        ChallengerModPlayer challengerModPlayer = Main.player[playerID].GetModPlayer<ChallengerModPlayer>();
        challengerModPlayer.ReceivePlayerSync(reader);

        if (Main.netMode == NetmodeID.Server) challengerModPlayer.SyncPlayer(-1, whoAmI, false);
    }

    private static void HandleTrueDamageText(BinaryReader reader)
    {
        int colorPacked = reader.ReadInt32();
        float x = reader.ReadSingle();
        float y = reader.ReadSingle();
        int damage = reader.ReadInt32();
        int ignorePlayer = reader.ReadInt32();

        x += Main.rand.NextFloat(-25f, 25f);
        y += Main.rand.NextFloat(-25f, 25f);

        if (Main.netMode == NetmodeID.Server)
            NetMessage.SendData(MessageID.CombatTextInt, number: colorPacked, number2: x, number3: y, number4: damage, ignoreClient: ignorePlayer, number5: 1, number6: 1, number7: 1);
    }

    private static void HandleTimeStop(BinaryReader reader)
    {
        byte whoActivated = reader.ReadByte();
        ChronoArmorPlayer.PacketTimeStop(whoActivated);
    }

    private static void HandleCharmOnKilled(BinaryReader reader, int whoAmI)
    {
        Vector2 pos = Vector2.Zero;
        pos.X = reader.ReadSingle();
        pos.Y = reader.ReadSingle();
        bool boss = reader.ReadBoolean();

        float luck = Main.player[whoAmI].GetModPlayer<StatPlayer>().GetTotalStat(ModContent.GetInstance<CharmLuckStat>());
        List<(CharmRarity, CharmType)> rolls = CharmsManager.RollForCharms(luck, boss: boss);
        CharmsManager.SpawnCharms(rolls, whoAmI, pos);
    }

    private void HandleSyncInvertedPlayer(BinaryReader reader, int whoAmI)
    {
        byte playerID = reader.ReadByte();

        InvertedModPlayer pPlayer = Main.player[playerID].GetModPlayer<InvertedModPlayer>();
        pPlayer.ReceiveSync(reader);

        if (Main.netMode == NetmodeID.Server)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)MessageType.SyncInvertedModPlayer);
            packet.Write(playerID);
            packet.Write(pPlayer.ManaSurgeMultiplier);
            packet.Send(-1, whoAmI);
        }
    }

    private void HandlePerceptiveCritEffect(BinaryReader reader, int whoAmI)
    {
        float x = reader.ReadSingle();
        float y = reader.ReadSingle();
        int damage = reader.ReadInt32();
        byte tier = reader.ReadByte();

        if (Main.netMode == NetmodeID.Server)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)MessageType.PerceptiveCritEffect);
            packet.Write(x);
            packet.Write(y);
            packet.Write(damage);
            packet.Write(tier);
            packet.Send(-1, whoAmI);
        }
        else
            WeaponUtils.SpawnPerceptiveText(new Vector2(x, y), damage, tier);
    }

    private static void HandleSilentTileKill(BinaryReader reader, int whoAmI)
    {
        int x = reader.ReadInt32();
        int y = reader.ReadInt32();
        bool isWall = reader.ReadBoolean();
        bool noItem = reader.ReadBoolean();

        if (Main.netMode != NetmodeID.Server) return;

        if (isWall)
            WorldGen.KillWall(x, y);
        else
            WorldGen.KillTile(x, y, false, false, noItem);

        NetMessage.SendData(MessageID.TileManipulation, -1, whoAmI, null, isWall ? 2 : 0, x, y);
    }

    private void HandleSyncFortuneModPlayer(BinaryReader reader)
    {
        byte playerID = reader.ReadByte();

        if (!Main.player[playerID].active) return;

        FortuneModPlayer fPlayer = Main.player[playerID].GetModPlayer<FortuneModPlayer>();
        fPlayer.ReceiveSync(reader);

        if (Main.netMode != NetmodeID.Server) return;

        ModPacket packet = Mod.GetPacket();
        packet.Write((byte)MessageType.SyncFortuneModPlayer);
        packet.Write(playerID);
        packet.Write(fPlayer.AxeFortune);
        packet.Send(-1, playerID);
    }

    private void HandleNegatedText(BinaryReader reader, int whoAmI)
    {
        byte targetPlayerID = reader.ReadByte();

        if (targetPlayerID >= Main.player.Length) return;

        Player target = Main.player[targetPlayerID];

        if (Main.netMode == NetmodeID.Server)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)MessageType.NegatedText);
            packet.Write(targetPlayerID);
            packet.Send(-1, whoAmI);
        }
        else if (target.active) CombatText.NewText(target.getRect(), Color.Green, "Negated!");
    }
}