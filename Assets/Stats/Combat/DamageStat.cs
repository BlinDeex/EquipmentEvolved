using System;
using EquipmentEvolved.Assets.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Stats.Combat;

public class DamageStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => 
        $"+{Math.Round(totalValue * 100)}% Damage";

    public override void PostUpdateEquips(Player player, float totalValue)
    {
        player.GetDamage<GenericDamageClass>() += totalValue;
    }
}