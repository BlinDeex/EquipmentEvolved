using System;
using EquipmentEvolved.Assets.Core;
using Terraria;

namespace EquipmentEvolved.Assets.Stats.Defense;

public class DamageReductionStat : EquipmentStat
{
    public override StatStackingMode StackingMode => StatStackingMode.Asymptotic;
    
    public override string FormatTooltip(float totalValue) => GetLocalization("Tooltip").Format(Math.Round(totalValue * 100));

    public override void ModifyHurt(Player player, ref Player.HurtModifiers modifiers, float totalValue)
    {
        float damageMul = 1f - totalValue;
        modifiers.FinalDamage *= damageMul;
    }
}