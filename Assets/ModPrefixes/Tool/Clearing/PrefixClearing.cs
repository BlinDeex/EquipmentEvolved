using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Tool.Clearing;

public class PrefixClearing : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;
    public override float ReforgeMultiplier => PrefixBalance.TOOL_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Pickaxe | SpecializedPrefixType.Hammer;

    public LocalizedText ClearingAreaImprovement { get; private set; }
    public LocalizedText ClearingChanceToLoseBlocks { get; private set; }

    protected override void OnSetStaticDefaults()
    {
         // Gets the normal description safely if you add one later!
        ClearingAreaImprovement = GetLoc(nameof(ClearingAreaImprovement));
        ClearingChanceToLoseBlocks = GetLoc(nameof(ClearingChanceToLoseBlocks));
    }

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "newLine", ClearingAreaImprovement.Format((int)PrefixBalance.CLEARING_MINING_AREA))
        {
            IsModifier = true
        };

        yield return new TooltipLine(Mod, "newLine2", ClearingChanceToLoseBlocks.Format(MathF.Round(PrefixBalance.CLEARING_CHANCE_TO_LOSE_MINED_BLOCK * 100, 2)))
        {
            IsModifier = true,
            IsModifierBad = true
        };
    }
}