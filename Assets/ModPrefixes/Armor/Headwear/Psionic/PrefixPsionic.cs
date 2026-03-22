using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Headwear.Psionic;

public class PrefixPsionic : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Headwear;

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        int healBonus = (int)((PrefixBalance.PSIONIC_HEAL_BONUS_MULT - 1f) * 100);

        yield return new TooltipLine(Mod, "PsionicDescription", Description.Format(healBonus))
        {
            IsModifier = true
        };
    }
}