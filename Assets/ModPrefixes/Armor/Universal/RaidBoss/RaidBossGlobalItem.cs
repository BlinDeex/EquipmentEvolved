using System;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Core;
using EquipmentEvolved.Assets.Stats.Defense;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.RaidBoss;

public class RaidBossGlobalItem : GlobalItem
{
    public override void SetStaticDefaults()
    {
        
        DefenseTooltipGlobalItem.DefenseModifiers.Add((item, _) =>
        {
            if (item.HasPrefix(ModContent.PrefixType<PrefixRaidBoss>()))
            {
                return (int)Math.Round(item.defense * (PrefixBalance.RAID_BOSS_PIECE_DEFENSE_MULT - 1f), MidpointRounding.AwayFromZero);
            }

            return 0;
        });
    }
    
    public override void UpdateEquip(Item item, Player player)
    {
        if (!item.HasPrefix(ModContent.PrefixType<PrefixRaidBoss>())) return;
        
        player.GetModPlayer<RaidBossModPlayer>().RaidBossPieces++;

        int bonusDefense = (int)Math.Round(item.defense * (PrefixBalance.RAID_BOSS_PIECE_DEFENSE_MULT - 1f), MidpointRounding.AwayFromZero);
        player.GetModPlayer<StatPlayer>().AddStat(ModContent.GetInstance<FlatDefenseStat>(), bonusDefense, StatSource.Armor);
    }
}