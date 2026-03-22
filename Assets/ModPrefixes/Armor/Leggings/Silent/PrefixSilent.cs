using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Leggings.Silent;

public class PrefixSilent : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Leggings;

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "SilentDesc", Description.Format(PrefixBalance.SILENT_AGGRO_REDUCTION))
        {
            IsModifier = true,
            IsModifierBad = false
        };
    }
}