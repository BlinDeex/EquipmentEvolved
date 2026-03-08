using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
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

        statPlayer.MovementSpeedMul -= PrefixBalance.RAID_BOSS_MOBILITY_PENALTY;
        statPlayer.WingHorizontalSpeed -= PrefixBalance.RAID_BOSS_MOBILITY_PENALTY;
        statPlayer.WingVerticalSpeed -= PrefixBalance.RAID_BOSS_MOBILITY_PENALTY;

        statPlayer.FlatDefense += PrefixBalance.RAID_BOSS_SET_DEFENSE_BONUS;
        statPlayer.DamageReductionMul += PrefixBalance.RAID_BOSS_SET_DR_BONUS;

        statPlayer.Aggro += PrefixBalance.RAID_BOSS_SET_AGGRO;
    }
}