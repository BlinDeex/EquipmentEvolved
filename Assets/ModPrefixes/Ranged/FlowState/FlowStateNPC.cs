using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.DEBUG;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.FlowState;

public class FlowStateNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public int FlowStacks;
    public int FlowTimer;
    public int BaseWeaponDamage;
    public int DecayTimer;

    // Used to track damage visually
    public float StoredFractionalDamage;
    public int TotalFlowDamage;
    
    // NEW: Variables to chunk our combat text
    public int ChunkedDamage;
    public int DamageTicker;

    public override void PostAI(NPC npc)
    {
        if (FlowStacks > 0)
        {
            // Handle Duration
            if (FlowTimer > 0)
            {
                FlowTimer--;
            }
            // Handle Rapid Decay
            else
            {
                DecayTimer++;
                if (DecayTimer >= PrefixBalance.FLOW_STATE_DECAY_RATE_TICKS)
                {
                    FlowStacks--;
                    DecayTimer = 0;
                }
            }

            // Clean up when stacks run out
            if (FlowStacks <= 0)
            {
                FlowStacks = 0;
                TotalFlowDamage = 0;
                StoredFractionalDamage = 0;
                ChunkedDamage = 0;
                DamageTicker = 0;
            }
        }
    }

    public override void UpdateLifeRegen(NPC npc, ref int damage)
    {
        if (FlowStacks > 0)
        {
            float rawDps = BaseWeaponDamage * PrefixBalance.FLOW_STATE_DPS_PERCENT_PER_STACK * FlowStacks;

            // Apply Terraria's standard defense mitigation (Boss defense / 2)
            // Note: In Master Mode, defense blocks 100% of its value, but for a DoT, 50% is a fair baseline.
            float mitigatedDps = rawDps - (npc.defense * 0.5f);
            
            // Ensure the bleed always does at least 1 damage per second if you have stacks
            if (mitigatedDps < 1f) mitigatedDps = 1f;

            float damagePerTick = mitigatedDps / 60f;
            StoredFractionalDamage += damagePerTick;

            if (StoredFractionalDamage >= 1f)
            {
                int dmg = (int)StoredFractionalDamage;
                StoredFractionalDamage -= dmg;
                TotalFlowDamage += dmg;
                ChunkedDamage += dmg; 

                npc.life -= dmg;

                if (npc.life <= 0 && Main.netMode != Terraria.ID.NetmodeID.MultiplayerClient)
                {
                    npc.checkDead();
                }
            }

            // (Combat text ticker remains exactly the same...)
            DamageTicker++;
            if (DamageTicker >= 30)
            {
                if (ChunkedDamage > 0)
                {
                    CombatText.NewText(npc.getRect(), Color.OrangeRed, ChunkedDamage);
                    ChunkedDamage = 0;
                }
                DamageTicker = 0;
            }
        }
    }

    public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        // Specifically allow the sticky text to draw on your Debug NPC, even though it's immortal!
        bool isDebugNPC = npc.type == ModContent.NPCType<DebugNPC>();

        if (FlowStacks > 0 && npc.life > 0 && !npc.isLikeATownNPC && (!npc.immortal || isDebugNPC))
        {
            float timeLeft = FlowTimer / 60f;

            // Color-coded text using Terraria's Chat Tags
            string text = $"[c/ff5555:{TotalFlowDamage}] [c/ffff55:x{FlowStacks}] [c/ffffff:{timeLeft:0.0}s]";

            // Measure the text so we can perfectly center it above the NPC
            Vector2 textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, text, Vector2.One);
            Vector2 pos = npc.Top - screenPos - new Vector2(textSize.X / 2f, 30);

            // Draw with a shadow outline for readability against busy backgrounds
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, pos, Color.White, 0f, Vector2.Zero, Vector2.One);
        }
    }
}