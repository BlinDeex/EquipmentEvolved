using System;
using System.Collections.Generic;
using System.Linq;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.RaidBoss;

public class RaidBossGlobalItem : GlobalItem
{
    public override void UpdateEquip(Item item, Player player)
    {
        if (item.HasPrefix(ModContent.PrefixType<PrefixRaidBoss>()))
        {
            player.GetModPlayer<RaidBossModPlayer>().RaidBossPieces++;

            int bonusDefense = (int)Math.Round(item.defense * (PrefixBalance.RAID_BOSS_PIECE_DEFENSE_MULT - 1f), MidpointRounding.AwayFromZero);
            player.GetModPlayer<StatPlayer>().FlatDefense += bonusDefense;
        }
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (!item.HasPrefix(ModContent.PrefixType<PrefixRaidBoss>())) return;

        int bonusDefense = (int)Math.Round(item.defense * (PrefixBalance.RAID_BOSS_PIECE_DEFENSE_MULT - 1f), MidpointRounding.AwayFromZero);

        if (bonusDefense <= 0) return;

        TooltipLine defenseLine = tooltips.FirstOrDefault(x => x.Name == "Defense" && x.Mod == "Terraria");

        if (defenseLine == null) return;

        string colorTag = $" [c/78BE78:(+{bonusDefense})]";
        string baseDefenseStr = item.defense.ToString();
        int insertIndex = defenseLine.Text.IndexOf(baseDefenseStr, StringComparison.Ordinal);

        if (insertIndex != -1) defenseLine.Text = defenseLine.Text.Insert(insertIndex + baseDefenseStr.Length, colorTag);
    }
}