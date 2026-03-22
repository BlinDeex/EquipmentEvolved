using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Chestplate.Rebounding;

public class PrefixRebounding : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory; // Update to your armor category!
    public override float ReforgeMultiplier => PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Chestplate;

    public LocalizedText ShatterWarning { get; private set; }

    protected override void OnSetStaticDefaults()
    {
         
        ShatterWarning = GetLoc(nameof(ShatterWarning));
    }

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        float defensePct = MathF.Round(PrefixBalance.REBOUNDING_DEFENSE_PER_STACK * 100f, 2);
        float durationSeconds = MathF.Round(PrefixBalance.REBOUNDING_STACK_DURATION / 60f, 2);
        float shrapnelPct = MathF.Round(PrefixBalance.REBOUNDING_DEFENSE_SCALING * 100f, 2);
        
        yield return new TooltipLine(Mod, "ReboundingDesc", Description.Format(defensePct, durationSeconds, shrapnelPct))
        {
            IsModifier = true,
            IsModifierBad = false
        };
        
        int maxStacks = PrefixBalance.REBOUNDING_MAX_STACKS;

        yield return new TooltipLine(Mod, "ReboundingWarning", ShatterWarning.Format(maxStacks))
        {
            IsModifier = true,
            IsModifierBad = true 
        };
    }
}