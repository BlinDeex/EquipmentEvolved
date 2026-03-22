using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Stats.Temporary;

public class ClearArmorCooldownStat : EquipmentStat
{
    public override StatStackingMode StackingMode => StatStackingMode.Max;
    public override string FormatTooltip(float totalValue) => string.Empty;

    public override void UpdateEquips(Player player, float totalValue)
    {
        if (!player.HasBuff<ArmorAbilityCooldownBuff>()) return;
        player.ClearBuff(player.FindBuffIndex(ModContent.BuffType<ArmorAbilityCooldownBuff>()));
    }
}