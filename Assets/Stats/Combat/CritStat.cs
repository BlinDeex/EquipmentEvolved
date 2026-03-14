using EquipmentEvolved.Assets.Core;
using Terraria;

namespace EquipmentEvolved.Assets.Stats.Combat;

public class CritStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue) => 
        $"+{totalValue}% Critical Strike Chance";

    public override void ModifyWeaponCrit(Player player, Item item, ref float crit, float totalValue)
    {
        crit += totalValue;
    }
}