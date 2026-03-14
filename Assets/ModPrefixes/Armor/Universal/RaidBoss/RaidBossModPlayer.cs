using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Stats.Defense;
using EquipmentEvolved.Assets.Stats.MobilityUtility;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.RaidBoss;

public class RaidBossModPlayer : ModPlayer
{
    public int RaidBossPieces;

    public bool RaidBossSetBonus => RaidBossPieces >= 3;

    public override void ResetEffects()
    {
        RaidBossPieces = 0;
    }

    public override void UpdateEquips()
    {
        if (!RaidBossSetBonus) return;

        StatPlayer statPlayer = Player.GetModPlayer<StatPlayer>();

        // Mobility Penalties (Passed as negative values)
        statPlayer.AddStat(ModContent.GetInstance<MoveSpeedStat>(), -PrefixBalance.RAID_BOSS_MOBILITY_PENALTY, StatSource.Armor);
        statPlayer.AddStat(ModContent.GetInstance<WingHorizontalSpeedStat>(), -PrefixBalance.RAID_BOSS_MOBILITY_PENALTY, StatSource.Armor);
        statPlayer.AddStat(ModContent.GetInstance<WingVerticalSpeedStat>(), -PrefixBalance.RAID_BOSS_MOBILITY_PENALTY, StatSource.Armor);

        // Defense and DR Buffs
        statPlayer.AddStat(ModContent.GetInstance<FlatDefenseStat>(), PrefixBalance.RAID_BOSS_SET_DEFENSE_BONUS, StatSource.Armor);
        statPlayer.AddStat(ModContent.GetInstance<DamageReductionStat>(), PrefixBalance.RAID_BOSS_SET_DR_BONUS, StatSource.Armor);

        // Aggro
        statPlayer.AddStat(ModContent.GetInstance<AggroStat>(), PrefixBalance.RAID_BOSS_SET_AGGRO, StatSource.Armor);
    }
}