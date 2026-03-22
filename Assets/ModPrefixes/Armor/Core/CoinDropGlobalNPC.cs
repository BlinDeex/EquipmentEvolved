using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Stats.MobilityUtility;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Core;

public class CoinDropGlobalNPC : GlobalNPC
{
    public override bool PreKill(NPC npc)
    {
        if (npc.lastInteraction == 255) return true;

        Player killer = Main.player[npc.lastInteraction];

        if (!killer.active || killer.dead) return true;

        StatPlayer statPlayer = killer.GetModPlayer<StatPlayer>();
        
        float coinMultiplier = 1f + statPlayer.GetTotalStat(ModContent.GetInstance<CoinDropMulStat>());
        
        npc.value *= coinMultiplier;

        return true;
    }
}