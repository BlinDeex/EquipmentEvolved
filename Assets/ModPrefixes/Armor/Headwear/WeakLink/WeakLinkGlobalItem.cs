using System;
using System.Collections.Generic;
using System.Linq;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Headwear.WeakLink;

public class WeakLinkGlobalItem : GlobalItem
{
    public override void UpdateEquip(Item item, Player player)
    {
        if (item.HasPrefix(ModContent.PrefixType<PrefixWeakLink>()))
        {
            int defenseLoss = (int)(item.defense * (1f - PrefixBalance.WEAK_LINK_THIS_DEFENSE_MULT));
            player.statDefense -= defenseLoss;

            Item chest = player.armor[1];
            Item legs = player.armor[2];
            int bonusDefense = 0;

            if (chest != null && !chest.IsAir) bonusDefense += (int)(chest.defense * (PrefixBalance.WEAK_LINK_OTHER_DEFENSE_MULT - 1f));

            if (legs != null && !legs.IsAir) bonusDefense += (int)(legs.defense * (PrefixBalance.WEAK_LINK_OTHER_DEFENSE_MULT - 1f));

            player.statDefense += bonusDefense;
        }
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (item.HasPrefix(ModContent.PrefixType<PrefixWeakLink>()))
        {
            int defenseLoss = (int)(item.defense * (1f - PrefixBalance.WEAK_LINK_THIS_DEFENSE_MULT));

            if (defenseLoss > 0)
            {
                TooltipLine defenseLine = tooltips.FirstOrDefault(x => x.Name == "Defense" && x.Mod == "Terraria");
                if (defenseLine != null)
                {
                    string colorTag = $" [c/BE7878:(-{defenseLoss})]";
                    string baseDefenseStr = item.defense.ToString();
                    int insertIndex = defenseLine.Text.IndexOf(baseDefenseStr, StringComparison.Ordinal);

                    if (insertIndex != -1) defenseLine.Text = defenseLine.Text.Insert(insertIndex + baseDefenseStr.Length, colorTag);
                }
            }
        }

        if (item.bodySlot != -1 || item.legSlot != -1)
        {
            Player player = Main.LocalPlayer;

            if (player.armor[0] == null || player.armor[0].IsAir || player.armor[0].prefix != ModContent.PrefixType<PrefixWeakLink>()) return;

            int bonusDefense = (int)(item.defense * (PrefixBalance.WEAK_LINK_OTHER_DEFENSE_MULT - 1f));

            if (bonusDefense <= 0) return;

            TooltipLine defenseLine = tooltips.FirstOrDefault(x => x.Name == "Defense" && x.Mod == "Terraria");

            if (defenseLine == null) return;

            string colorTag = $" [c/78BE78:(+{bonusDefense})]";
            string baseDefenseStr = item.defense.ToString();
            int insertIndex = defenseLine.Text.IndexOf(baseDefenseStr, StringComparison.Ordinal);

            if (insertIndex != -1) defenseLine.Text = defenseLine.Text.Insert(insertIndex + baseDefenseStr.Length, colorTag);
        }
    }
}