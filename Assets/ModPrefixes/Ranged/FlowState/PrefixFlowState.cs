using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.FlowState;

public class PrefixFlowState : ModPrefix, ISpecializedPrefix
{
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.RangedWeapon;
    public override PrefixCategory Category => PrefixCategory.Ranged;

    public static LocalizedText Desc { get; private set; }
    
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this, "FlowState", "DisplayName");

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }

    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this, "FlowState", nameof(Desc));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        // Convert duration to seconds
        float durationSeconds = PrefixBalance.FLOW_STATE_DURATION_TICKS / 60f;
        
        // Convert the float (e.g., 0.20f) to a nice percentage (20)
        int damagePercentage = (int)Math.Round(PrefixBalance.FLOW_STATE_DPS_PERCENT_PER_STACK * 100);

        yield return new TooltipLine(Mod, "FlowStateDesc", Desc.Format(durationSeconds, damagePercentage))
        {
            IsModifier = true,
            IsModifierBad = false
        };
    }
}