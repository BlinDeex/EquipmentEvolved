using System;
using EquipmentEvolved.Assets.Core;
using Terraria;

namespace EquipmentEvolved.Assets.Stats.Temporary;

public class InvincibilityStat : EquipmentStat
{
    public override StatStackingMode StackingMode => StatStackingMode.Max;

    public override string FormatTooltip(float totalValue) => GetLocalization("Tooltip").Format(MathF.Round(totalValue, 2));

    public override void PostUpdateEquips(Player player, float totalValue)
    {
        if (totalValue <= 0) return;

        player.immune = true;
        
        if (player.immuneTime < 2)
        {
            player.immuneTime = 2;
        }
        
        player.lavaImmune = true;
        player.fireWalk = true;
    }
    
    public override bool FreeDodge(Player player, Player.HurtInfo info, float totalValue)
    {
        return totalValue > 0;
    }
}