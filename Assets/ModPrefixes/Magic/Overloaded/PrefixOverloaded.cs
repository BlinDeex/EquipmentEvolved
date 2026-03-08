using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Magic.Overloaded;

public class PrefixOverloaded : ModPrefix, ISpecializedPrefix, IExperimentalPrefix // TODO add sound effect upon release
{
    public override PrefixCategory Category => PrefixCategory.Magic;

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Overloaded", "DisplayName");

    public static LocalizedText Description { get; private set; }
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.MagicWeapon;

    public override void SetStaticDefaults()
    {
        Description = LocalizationManager.GetPrefixLocalization(this, "Overloaded", nameof(Description));
    }

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        int damageEfficiency = (int)(PrefixBalance.OVERLOADED_DAMAGE_EFFICIENCY * 100);

        string formattedDescription = Description.Format(damageEfficiency);

        yield return new TooltipLine(Mod, "OverloadedDescription", formattedDescription)
        {
            OverrideColor = Color.Magenta
        };
    }
}