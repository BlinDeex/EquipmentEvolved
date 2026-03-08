using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Headwear.WeakLink;

public class PrefixWeakLink : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "WeakLink", "DisplayName");

    public static LocalizedText Description { get; private set; }
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Headwear;

    public override void SetStaticDefaults()
    {
        Description = LocalizationManager.GetPrefixLocalization(this, "WeakLink", nameof(Description));
    }

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    }

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