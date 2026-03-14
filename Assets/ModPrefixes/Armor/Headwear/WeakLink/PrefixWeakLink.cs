using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Headwear.WeakLink;

public class PrefixWeakLink : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Headwear;

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        int selfLoss = (int)((1f - PrefixBalance.WEAK_LINK_THIS_DEFENSE_MULT) * 100);
        int otherGain = (int)((PrefixBalance.WEAK_LINK_OTHER_DEFENSE_MULT - 1f) * 100);

        yield return new TooltipLine(Mod, "WeakLinkDescription", Description.Format(selfLoss, otherGain))
        {
            OverrideColor = Color.LightSlateGray
        };
    }
}