using EquipmentEvolved.Assets.Core;
using Terraria;

namespace EquipmentEvolved.Assets.Stats.Defense;

public class IframesStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => 
        $"+{(int)totalValue} Invincibility Frames on Hit";

    public override void PostHurt(Player player, Player.HurtInfo info, float totalValue)
    {
        player.immuneTime += (int)totalValue;
    }
}