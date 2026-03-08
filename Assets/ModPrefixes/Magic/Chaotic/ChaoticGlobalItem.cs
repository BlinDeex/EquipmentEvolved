using System.Collections.Generic;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Magic.Chaotic;

public class ChaoticGlobalItem : GlobalItem
{
    public static List<(string Name, string Text, Color Color)> CurrentTooltipData { get; set; } = [];

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (!item.HasPrefix(ModContent.PrefixType<PrefixChaotic>())) return;

        foreach ((string Name, string Text, Color Color) data in CurrentTooltipData)
        {
            tooltips.Add(new TooltipLine(Mod, data.Name, data.Text)
            {
                OverrideColor = data.Color
            });
        }
    }
}