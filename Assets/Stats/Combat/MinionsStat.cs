using EquipmentEvolved.Assets.Core;
using Terraria;

namespace EquipmentEvolved.Assets.Stats.Combat;

public class MinionsStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => 
        $"+{(int)totalValue} Max Minions";

    public override void PostUpdateEquips(Player player, float totalValue)
    {
        player.maxMinions += (int)totalValue;
    }
}