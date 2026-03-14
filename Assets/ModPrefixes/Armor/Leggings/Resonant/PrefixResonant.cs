using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Leggings.Resonant;

public class PrefixResonant : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Leggings;

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        int boost = (int)(PrefixBalance.RESONANT_PREFIX_BOOST * 100);

        yield return new TooltipLine(Mod, "ResonantDescription", Description.Format(boost))
        {
            OverrideColor = Color.HotPink
        };
    }
}