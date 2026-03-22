using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Headwear.Berserker;

public class PrefixBerserker : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Headwear;

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        int maxBonus = (int)(PrefixBalance.BERSERKER_MAX_DAMAGE_BONUS * 100);

        yield return new TooltipLine(Mod, "BerserkerDescription", Description.Format(maxBonus))
        {
            IsModifier = true
        };
    }
}