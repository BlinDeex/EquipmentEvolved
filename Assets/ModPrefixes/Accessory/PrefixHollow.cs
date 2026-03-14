using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Stats.Defense;
using EquipmentEvolved.Assets.Stats.MobilityUtility;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Accessory;

public class PrefixHollow : BaseEvolvedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ACCESSORY_REFORGING_MULTIPLIER;

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        int healthDec = (int)((1f - PrefixBalance.HOLLOW_MAX_HEALTH_MULT) * 100);
        int mobInc = (int)((PrefixBalance.HOLLOW_MOBILITY_MULT - 1f) * 100);

        yield return new TooltipLine(Mod, "newLine", Description.Format(mobInc, healthDec))
        {
            OverrideColor = Color.LightSlateGray,
            IsModifier = true
        };
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        StatPlayer statPlayer = player.GetModPlayer<StatPlayer>();

        float healthBonus = PrefixBalance.HOLLOW_MAX_HEALTH_MULT - 1f;
        statPlayer.AddStat(ModContent.GetInstance<HealthCapMulStat>(), healthBonus, StatSource.Accessory);

        float baseMobility = PrefixBalance.HOLLOW_MOBILITY_MULT - 1f;
        statPlayer.AddStat(ModContent.GetInstance<MoveSpeedStat>(), baseMobility, StatSource.Accessory);
        statPlayer.AddStat(ModContent.GetInstance<WingHorizontalAccStat>(), baseMobility, StatSource.Accessory);
        statPlayer.AddStat(ModContent.GetInstance<WingHorizontalSpeedStat>(), baseMobility, StatSource.Accessory);
        statPlayer.AddStat(ModContent.GetInstance<WingVerticalAccStat>(), baseMobility, StatSource.Accessory);
        statPlayer.AddStat(ModContent.GetInstance<WingVerticalSpeedStat>(), baseMobility, StatSource.Accessory);
    }
}