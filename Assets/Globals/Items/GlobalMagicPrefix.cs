using System.Collections.Generic;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Globals.Items;

public class GlobalMagicPrefix : GlobalItem
{
    public static List<TooltipLine> CurrentChaoticTooltipLines { get; set; } = new();
}