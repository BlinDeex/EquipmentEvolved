using System;
using System.Collections.Generic;
using System.IO;
using EquipmentEvolved.Assets.CharmsModule;
using EquipmentEvolved.Assets.CharmsModule.Manager;
using EquipmentEvolved.Assets.ModPlayers;
using EquipmentEvolved.Assets.ModPlayers.Armor.Universal;
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

        // The switch statement now simply routes to the correct method
        switch (messageType)
        {
            case MessageType.ChallengerScore:
                HandleChallengerScore(reader, whoAmI);
                break;
            case MessageType.TrueDamageText:
                HandleTrueDamageText(reader, whoAmI);
                break;
            case MessageType.TimeStop:
                HandleTimeStop(reader);
                break;
            case MessageType.CharmOnKilled:
                HandleCharmOnKilled(reader, whoAmI);
                break;
            case MessageType.SyncPrefixPlayer:
                HandleSyncPrefixPlayer(reader, whoAmI);
                break;
            case MessageType.PerceptiveCritEffect:
                HandlePerceptiveCritEffect(reader, whoAmI);
                break;
            case MessageType.SilentTileKill:
                HandleSilentTileKill(reader, whoAmI);
                break;
            case MessageType.SyncToolPlayer:
                HandleSyncToolPlayer(reader, whoAmI);
                break;
            case MessageType.NegatedText:
                HandleNegatedText(reader, whoAmI);
                break;
            default:
                throw new ArgumentOutOfRangeException($"Unhandled MessageType: {messageType}");
        }
    }

    // --- Packet Handler Methods ---

    private void HandleChallengerScore(BinaryReader reader, int whoAmI)
    {
        byte playerID = reader.ReadByte();
        ChallengerPlayer challengerPlayer = Main.player[playerID].GetModPlayer<ChallengerPlayer>();
        challengerPlayer.ReceivePlayerSync(reader);
        
        if (Main.netMode == NetmodeID.Server)
        {
            challengerPlayer.SyncPlayer(-1, whoAmI, false);
        }
    }

    private void HandleTrueDamageText(BinaryReader reader, int whoAmI)
    {
        int colorPacked = reader.ReadInt32();
        float x = reader.ReadSingle();
        float y = reader.ReadSingle();
        int damage = reader.ReadInt32();
        int ignorePlayer = reader.ReadInt32();

        // Apply vanilla-style random offset to prevent exact overlapping
        x += Main.rand.NextFloat(-25f, 25f);
        y += Main.rand.NextFloat(-25f, 25f);

        // Server relays the text to everyone else with the jittered coordinates
        if (Main.netMode == NetmodeID.Server)
        {
            NetMessage.SendData(MessageID.CombatTextInt, number: colorPacked, number2: x, number3: y,
                number4: damage, ignoreClient: ignorePlayer, number5: 1, number6: 1, number7: 1);
        }
    }

    private void HandleTimeStop(BinaryReader reader)
    {
        byte whoActivated = reader.ReadByte();
        ChronoArmorPlayer.PacketTimeStop(whoActivated);
    }

    private void HandleCharmOnKilled(BinaryReader reader, int whoAmI)
    {
        Vector2 pos = Vector2.Zero;
        pos.X = reader.ReadSingle();
        pos.Y = reader.ReadSingle();
        bool boss = reader.ReadBoolean();
        
        float luck = Main.player[whoAmI].GetModPlayer<StatPlayer>().CharmLuckMul;
        List<(CharmRarity, CharmType)> rolls = CharmsManager.RollForCharms(luck, boss: boss);
        CharmsManager.SpawnCharms(rolls, whoAmI, spawnPos: pos);
    }

    private void HandleSyncPrefixPlayer(BinaryReader reader, int whoAmI)
    {
        byte playerID = reader.ReadByte();
        PrefixPlayer pPlayer = Main.player[playerID].GetModPlayer<PrefixPlayer>();
        pPlayer.ReceiveSync(reader);

        if (Main.netMode == NetmodeID.Server)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)MessageType.SyncPrefixPlayer);
            packet.Write(playerID);
            packet.Write(pPlayer.ManaSurgeMultiplier);
            packet.Send(-1, whoAmI); // Send to everyone except sender
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
        {
            WeaponUtils.SpawnPerceptiveText(new Vector2(x, y), damage, tier);
        }
    }

    private void HandleSilentTileKill(BinaryReader reader, int whoAmI)
    {
        int x = reader.ReadInt32();
        int y = reader.ReadInt32();
        bool isWall = reader.ReadBoolean();
        bool noItem = reader.ReadBoolean();

        if (Main.netMode == NetmodeID.Server)
        {
            // Execute on Server
            if (isWall)
                WorldGen.KillWall(x, y, fail: false);
            else
                WorldGen.KillTile(x, y, fail: false, effectOnly: false, noItem: noItem);

            // Forward to other Clients
            NetMessage.SendData(MessageID.TileManipulation, -1, whoAmI, null, isWall ? 2 : 0, x, y);
        }
    }

    private void HandleSyncToolPlayer(BinaryReader reader, int whoAmI)
    {
        byte playerID = reader.ReadByte();

        if (Main.player[playerID].active)
        {
            ToolPlayer tPlayer = Main.player[playerID].GetModPlayer<ToolPlayer>();
            tPlayer.ReceiveSync(reader);

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MessageType.SyncToolPlayer);
                packet.Write(playerID);
                packet.Write(tPlayer.AxeFortune);
                packet.Send(-1, playerID);
            }
        }
    }

    private void HandleNegatedText(BinaryReader reader, int whoAmI)
    {
        byte targetPlayerID = reader.ReadByte();

        if (targetPlayerID < Main.player.Length)
        {
            Player target = Main.player[targetPlayerID];

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MessageType.NegatedText);
                packet.Write(targetPlayerID);
                packet.Send(-1, whoAmI);
            }
            else if (target.active)
            {
                CombatText.NewText(target.getRect(), Color.Green, "Negated!");
            }
        }
    }
}