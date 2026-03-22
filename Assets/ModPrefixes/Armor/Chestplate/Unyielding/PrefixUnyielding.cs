using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Chestplate.Unyielding;

public class PrefixUnyielding : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Chestplate;

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        int drPercent = (int)Math.Round(PrefixBalance.UNYIELDING_MAX_DR * 100);
        yield return new TooltipLine(Mod, "UnyieldingDesc", Description.Format(drPercent)) { IsModifier = true };
    }
}