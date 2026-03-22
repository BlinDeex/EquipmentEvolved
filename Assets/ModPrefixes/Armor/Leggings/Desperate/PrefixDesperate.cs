using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Leggings.Desperate;

public class PrefixDesperate : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Leggings;

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        int speedPercent = (int)Math.Round(PrefixBalance.DESPERATE_MAX_SPEED_BONUS * 100);

        yield return new TooltipLine(Mod, "DesperateDesc", Description.Format(speedPercent))
        {
            IsModifier = true,
            IsModifierBad = false
        };
    }
}