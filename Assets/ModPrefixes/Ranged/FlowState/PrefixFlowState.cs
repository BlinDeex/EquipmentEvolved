using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.FlowState;

public class PrefixFlowState : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Ranged;
    public override float ReforgeMultiplier => PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.RangedWeapon;

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        float durationSeconds = PrefixBalance.FLOW_STATE_DURATION_TICKS / 60f;
        int damagePercentage = (int)Math.Round(PrefixBalance.FLOW_STATE_DPS_PERCENT_PER_STACK * 100);

        yield return new TooltipLine(Mod, "FlowStateDesc", Description.Format(durationSeconds, damagePercentage))
        {
            IsModifier = true
        };
    }
}