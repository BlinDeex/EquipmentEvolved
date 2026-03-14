using System;
using EquipmentEvolved.Assets.Core;
using Terraria;

namespace EquipmentEvolved.Assets.Stats.Defense;

public class DamageReductionStat : EquipmentStat
{
    public override StatStackingMode StackingMode => StatStackingMode.Asymptotic;
    
    public override string FormatTooltip(float totalValue) => 
        $"+{Math.Round(totalValue * 100)}% Damage Reduction";

    public override void ModifyHurt(Player player, ref Player.HurtModifiers modifiers, float totalValue)
    {
        // 0.15 totalValue means 15% damage reduction (0.85 multiplier)
        float damageMul = 1f - totalValue;
        
        if (damageMul <= 0.1f) damageMul = 0.1f; // Maximum 90% DR cap

        modifiers.FinalDamage *= damageMul;
    }
}