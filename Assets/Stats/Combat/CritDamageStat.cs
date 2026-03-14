using System;
using EquipmentEvolved.Assets.Core;
using Terraria;

namespace EquipmentEvolved.Assets.Stats.Combat;

public class CritDamageStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => 
        $"+{Math.Round(totalValue * 100)}% Critical Strike Damage";

    public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers, float totalValue)
    {
        modifiers.CritDamage += totalValue;
    }
}