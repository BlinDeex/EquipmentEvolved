using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Axe.Fortune;

public class PrefixFortune : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;
    public override float ReforgeMultiplier => PrefixBalance.TOOL_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Axe;

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "newLine", Description.Value)
        {
            IsModifier = true
        };
    }
}