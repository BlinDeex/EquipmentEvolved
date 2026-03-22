using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Pickaxe.Revealing;

public class PrefixRevealing : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;
    public override float ReforgeMultiplier => PrefixBalance.TOOL_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Pickaxe;

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "newLine", Description.Format(MathF.Round(PrefixBalance.REVEALING_CHANCE * 100, 2)))
        {
            IsModifier = true
        };
    }
}