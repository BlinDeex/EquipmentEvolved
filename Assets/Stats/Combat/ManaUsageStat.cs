using System;
using EquipmentEvolved.Assets.Core;
using Terraria;

namespace EquipmentEvolved.Assets.Stats.Combat;

public class ManaUsageStat : EquipmentStat
{
    public override string FormatTooltip(float totalValue)
    {
        // If it's a negative value (like -0.15 for 15% reduction), we format it nicely
        int percent = (int)Math.Round(totalValue * 100);
        return percent < 0 ? $"{percent}% Mana Cost" : $"+{percent}% Mana Cost";
    }

    public override void ModifyManaCost(Player player, Item item, ref float reduce, ref float mult, float totalValue)
    {
        // Base is 1f, totalValue modifies it (e.g., 1f - 0.15f = 0.85f mult)
        mult *= Math.Clamp(1f + totalValue, 0f, 10f);
    }
}