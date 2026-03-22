using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Utilities;
using Terraria;

namespace EquipmentEvolved.Assets.Stats.MobilityUtility;

public class CoinDropOnHitStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => GetLocalization("Tooltip").Format((int)totalValue);
    
    public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone, float totalValue)
    {
        if (totalValue <= 0 || target.life <= 0) return; 
        CombatUtils.DropCoins(totalValue, target);
    }
}