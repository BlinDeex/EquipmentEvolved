using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.DEBUG;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.FlowState;

public class FlowStateNPC : GlobalNPC
{
    public int BaseWeaponDamage;
    
    public int ChunkedDamage;
    public int DamageTicker;
    public int DecayTimer;

    public int FlowStacks;
    public int FlowTimer;
    
    public float StoredFractionalDamage;
    public int TotalFlowDamage;
    public override bool InstancePerEntity => true;

    public override void PostAI(NPC npc)
    {
        if (FlowStacks <= 0) return;
        
        if (FlowTimer > 0)
            FlowTimer--;
        else
        {
            DecayTimer++;
            if (DecayTimer >= PrefixBalance.FLOW_STATE_DECAY_RATE_TICKS)
            {
                FlowStacks--;
                DecayTimer = 0;
            }
        }

        if (FlowStacks > 0) return;
        
        FlowStacks = 0;
        TotalFlowDamage = 0;
        StoredFractionalDamage = 0;
        ChunkedDamage = 0;
        DamageTicker = 0;
    }

    public override void UpdateLifeRegen(NPC npc, ref int damage)
    {
        if (FlowStacks <= 0) return;
        float rawDps = BaseWeaponDamage * PrefixBalance.FLOW_STATE_DPS_PERCENT_PER_STACK * FlowStacks;
        
        float mitigatedDps = rawDps - npc.defense * 0.5f;
        
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

            if (npc.life <= 0 && Main.netMode != NetmodeID.MultiplayerClient) npc.checkDead();
        }
        
        DamageTicker++;
        if (DamageTicker < 30) return;
        
        if (ChunkedDamage > 0)
        {
            CombatText.NewText(npc.getRect(), Color.OrangeRed, ChunkedDamage);
            ChunkedDamage = 0;
        }

        DamageTicker = 0;
    }

    public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        bool isDebugNPC = npc.type == ModContent.NPCType<DebugNPC>();

        if (FlowStacks <= 0 || npc.life <= 0 || npc.isLikeATownNPC || (npc.immortal && !isDebugNPC)) return;
        
        float timeLeft = FlowTimer / 60f;
        
        string text = $"[c/ff5555:{TotalFlowDamage}] [c/ffff55:x{FlowStacks}] [c/ffffff:{timeLeft:0.0}s]";
        
        Vector2 textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, text, Vector2.One);
        Vector2 pos = npc.Top - screenPos - new Vector2(textSize.X / 2f, 30);
        
        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, pos, Color.White, 0f, Vector2.Zero, Vector2.One);
    }
}