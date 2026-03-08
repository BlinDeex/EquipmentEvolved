using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Headwear.Berserker;

public class PrefixBerserker : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Berserker", "DisplayName");

    public static LocalizedText Description { get; private set; }
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Headwear;

    public override void SetStaticDefaults()
    {
        Description = LocalizationManager.GetPrefixLocalization(this, "Berserker", nameof(Description));
    }

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        int maxBonus = (int)(PrefixBalance.BERSERKER_MAX_DAMAGE_BONUS * 100);

        yield return new TooltipLine(Mod, "BerserkerDescription", Description.Format(maxBonus))
        {
            OverrideColor = Color.DarkRed
        };
    }
}