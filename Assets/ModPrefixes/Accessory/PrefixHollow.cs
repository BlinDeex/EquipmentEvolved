using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Stats.Defense;
using EquipmentEvolved.Assets.Stats.MobilityUtility;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Accessory;

public class PrefixHollow : BaseEvolvedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ACCESSORY_REFORGING_MULTIPLIER;
    
    public LocalizedText HealthDec { get; private set; }

    protected override void OnSetStaticDefaults()
    {
        HealthDec = GetLoc(nameof(HealthDec));
    }

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        float healthDec = MathF.Round((1f - PrefixBalance.HOLLOW_MAX_HEALTH_MULT) * 100, 2);
        float mobInc = MathF.Round((PrefixBalance.HOLLOW_MOBILITY_MULT - 1f) * 100, 2);
        
        yield return new TooltipLine(Mod, "newLine", Description.Format(mobInc))
        {
            IsModifier = true
        };
        yield return new TooltipLine(Mod, "newLine", Description.Format(healthDec))
        {
            IsModifier = true,
            IsModifierBad = true
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