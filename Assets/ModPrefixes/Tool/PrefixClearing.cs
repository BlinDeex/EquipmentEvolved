using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Tool;

public class PrefixClearing : ModPrefix, ISpecializedPrefix
{
    public SpecializedPrefixType SpecializedPrefixType =>
        SpecializedPrefixType.Pickaxe | SpecializedPrefixType.Hammer;

    public override PrefixCategory Category => PrefixCategory.AnyWeapon;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.TOOL_REFORGING_MULTIPLIER;
    }

    public static LocalizedText ClearingAreaImprovement { get; private set; }
    public static LocalizedText ClearingChanceToLoseBlocks { get; private set; }
    
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"Clearing", "DisplayName");


    public override void SetStaticDefaults()
    {
        ClearingAreaImprovement = LocalizationManager.GetPrefixLocalization(this,"Clearing", nameof(ClearingAreaImprovement));
        ClearingChanceToLoseBlocks = LocalizationManager.GetPrefixLocalization(this,"Clearing", nameof(ClearingChanceToLoseBlocks));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        TooltipLine newLine = new TooltipLine(Mod, "newLine",
            ClearingAreaImprovement.Value)
        {
            IsModifier = true
        };

        TooltipLine newLine2 = new TooltipLine(Mod, "newLine2",
            ClearingChanceToLoseBlocks.Format((int)(PrefixBalance.CLEARING_CHANCE_TO_LOSE_MINED_BLOCK * 100)))
        {
            IsModifier = true,
            IsModifierBad = true
        };

        yield return newLine;
        yield return newLine2;
    }
}