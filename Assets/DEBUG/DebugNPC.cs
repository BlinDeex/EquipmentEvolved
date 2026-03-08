using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.DEBUG;

public class DebugNPC : ModNPC
{
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 1;
    }

    public override void SetDefaults()
    {
        NPC.width = 32;
        NPC.height = 32;
        NPC.damage = 0;
        NPC.defense = 100;
        NPC.lifeMax = 9999999;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.knockBackResist = 0f;
        NPC.aiStyle = -1;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.immortal = true;
    }
}