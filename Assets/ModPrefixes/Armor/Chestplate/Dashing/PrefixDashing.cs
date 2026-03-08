using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Chestplate.Dashing;

public class PrefixDashing : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Dashing", "DisplayName");

    public static LocalizedText Description { get; private set; }
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Chestplate;

    public override void SetStaticDefaults()
    {
        Description = LocalizationManager.GetPrefixLocalization(this, "Dashing", nameof(Description));
    }

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "DashingDescription", Description.Format(PrefixBalance.DASHING_MAX_CHARGES))
        {
            OverrideColor = Color.Cyan
        };
    }
}