using System;
using EquipmentEvolved.Assets.Core;
using Terraria;

namespace EquipmentEvolved.Assets.Stats.Combat;

public class CritDamageStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => GetLocalization("Tooltip").Format(Math.Round(totalValue * 100, 2));

    public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers, float totalValue)
    {
        modifiers.CritDamage += totalValue;
    }
}