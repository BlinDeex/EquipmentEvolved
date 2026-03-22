using System;
using EquipmentEvolved.Assets.Core;
using Terraria;

namespace EquipmentEvolved.Assets.Stats.Defense;

public class IframesStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => GetLocalization("Tooltip").Format(Math.Round(totalValue));

    public override void PostHurt(Player player, Player.HurtInfo info, float totalValue)
    {
        player.immuneTime += (int)Math.Round(totalValue);
    }
}