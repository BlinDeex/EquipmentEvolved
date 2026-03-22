using System;
using EquipmentEvolved.Assets.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Stats.Combat;

public class RangedDamageStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => GetLocalization("Tooltip").Format(Math.Round(totalValue * 100));

    public override void PostUpdateEquips(Player player, float totalValue)
    {
        player.GetDamage<RangedDamageClass>() += totalValue;
    }
}