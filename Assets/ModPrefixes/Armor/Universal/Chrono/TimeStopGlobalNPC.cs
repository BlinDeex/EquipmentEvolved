using System.Collections.Generic;
using System.IO;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.Chrono;

public class TimeStopGlobalNPC : GlobalNPC
{
    private bool timeStopActive;

    private int timeStopTicks;
    public override bool InstancePerEntity => true;

    public override bool PreAI(NPC npc)
    {
        bool runAI = true;
        TickTimeStop(ref runAI, npc);

        return runAI;
    }

    private void TickTimeStop(ref bool runAI, NPC npc)
    {
        if (!timeStopActive) return;

        if (timeStopTicks <= 0)
        {
            timeStopActive = false;
            npc.netUpdate = true;
            return;
        }

        npc.position = npc.oldPosition;
        npc.direction = npc.oldDirection;
        npc.velocity = Vector2.Zero;
        npc.frameCounter = 0.0;
        npc.aiAction = 0;
        npc.timeLeft++;

        timeStopTicks--;

        runAI = false;
    }

    public void TimeStop(int ticks, NPC npc)
    {
        if (timeStopActive) return;

        timeStopTicks = ticks;
        timeStopActive = true;
        npc.netUpdate = true;

        if (!CombatUtils.TryFindSegments(npc, out List<NPC> segments)) return;

        foreach (NPC segment in segments)
        {
            segment.GetGlobalNPC<TimeStopGlobalNPC>().TimeStop(ticks, segment);
        }
    }

    public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
    {
        return !timeStopActive;
    }

    public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
    {
        bitWriter.WriteBit(timeStopActive);
        binaryWriter.Write(timeStopTicks);
    }

    public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
    {
        timeStopActive = bitReader.ReadBit();
        timeStopTicks = binaryReader.ReadInt32();
    }
}