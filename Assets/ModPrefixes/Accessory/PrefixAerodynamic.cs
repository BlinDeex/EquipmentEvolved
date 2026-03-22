using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Stats.MobilityUtility;
using Terraria;
using Terraria.ModLoader;
// using EquipmentEvolved.Assets.Core.CoreNew; <-- Add this if StatPlayer is in the new namespace

namespace EquipmentEvolved.Assets.ModPrefixes.Accessory;

public class PrefixAerodynamic : BaseEvolvedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ACCESSORY_REFORGING_MULTIPLIER;

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "newLine", Description.Format(PrefixBalance.AERODYNAMIC_MOVEMENT_MULTIPLIER * 100, PrefixBalance.AERODYNAMIC_WING_TIME_TICKS))
        {
            IsModifier = true
        };
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        StatPlayer statPlayer = player.GetModPlayer<StatPlayer>();
        statPlayer.AddStat(ModContent.GetInstance<MoveSpeedStat>(), PrefixBalance.AERODYNAMIC_MOVEMENT_MULTIPLIER, StatSource.Accessory);
        statPlayer.AddStat(ModContent.GetInstance<WingTimeStat>(), PrefixBalance.AERODYNAMIC_WING_TIME_TICKS, StatSource.Accessory);
    }
}