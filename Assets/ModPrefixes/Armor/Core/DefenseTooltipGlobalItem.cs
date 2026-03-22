using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Core;

public class DefenseTooltipGlobalItem : GlobalItem
{
    public static List<Func<Item, Player, int>> DefenseModifiers = new();

    public override void Unload()
    {
        DefenseModifiers.Clear();
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (item.defense <= 0) return;

        Player player = Main.LocalPlayer;

        int netDefenseModifier = DefenseModifiers.Sum(modifierFunc => modifierFunc.Invoke(item, player));

        if (netDefenseModifier == 0) return;

        TooltipLine defenseLine = tooltips.FirstOrDefault(x => x.Name == "Defense" && x.Mod == "Terraria");
        if (defenseLine == null) return;

        int totalDefense = item.defense + netDefenseModifier;
        
        string colorTag = netDefenseModifier > 0 
            ? $" [c/78BE78:(+{netDefenseModifier})]" 
            : $" [c/BE7878:({netDefenseModifier})]";

        string baseDefenseStr = item.defense.ToString();
        int insertIndex = defenseLine.Text.IndexOf(baseDefenseStr, StringComparison.Ordinal);

        if (insertIndex != -1)
        {
            defenseLine.Text = defenseLine.Text.Remove(insertIndex, baseDefenseStr.Length).Insert(insertIndex, $"{totalDefense}{colorTag}");
        }
    }
}