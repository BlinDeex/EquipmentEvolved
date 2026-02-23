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
        NPC.damage = 0; // Won't hurt you
        NPC.defense = 0; // 0 defense so you see raw weapon damage
        NPC.lifeMax = 9999999;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.knockBackResist = 0f; // Stays perfectly still when hit
        NPC.aiStyle = -1; // Absolutely no AI
        NPC.noGravity = true; // Floats exactly where you click with Cheat Sheet
        NPC.noTileCollide = true; // Prevents it from falling through blocks if spawned weirdly
        
        // This is a native Terraria flag that allows it to take hits and display damage numbers,
        // but prevents its health from dropping to 0 naturally.
        NPC.immortal = true; 
    }

    // Ultimate failsafe: If you deal 10 million damage in one hit, 
    // this completely cancels the death sequence and heals it back to full.
    public override bool CheckDead()
    {
        NPC.life = NPC.lifeMax;
        return false; 
    }
}