using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ModLoader;

// Assuming this is where WeaponUtils is!

namespace EquipmentEvolved.Assets.Stats.Custom;

public class TrueDamageFlatStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => 
        $"+{(int)totalValue} True Damage";

    public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone, float totalValue)
    {
        if (totalValue <= 0) return;
        
        float mul = 1f + player.GetModPlayer<StatPlayer>().GetTotalStat(ModContent.GetInstance<TrueDamageMulStat>());
        WeaponUtils.InflictTrueDamage(target, totalValue * mul);
    }
}