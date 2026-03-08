using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.Sundering;

public class SunderingGlobalNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public float SunderedDefense { get; set; }

    public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
    {
        if (SunderedDefense > 0) modifiers.ArmorPenetration += SunderedDefense;
    }

    public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
    {
        binaryWriter.Write(SunderedDefense);
    }

    public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
    {
        SunderedDefense = binaryReader.ReadSingle();
    }
}