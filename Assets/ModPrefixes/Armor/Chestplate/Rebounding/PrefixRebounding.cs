using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Chestplate.Rebounding;

public class PrefixRebounding : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Chestplate;

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        int scalingPercent = (int)Math.Round(PrefixBalance.REBOUNDING_DEFENSE_SCALING * 100);
        yield return new TooltipLine(Mod, "ReboundingDesc", Description.Format(scalingPercent, PrefixBalance.REBOUNDING_MAX_STACKS)) { IsModifier = true };
    }
}