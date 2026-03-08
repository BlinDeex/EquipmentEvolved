using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.Payback;

public class PrefixPayback : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Payback", "DisplayName");

    public static LocalizedText Description { get; private set; }

    public SpecializedPrefixType SpecializedPrefixType =>
        SpecializedPrefixType.MeleeWeapon | SpecializedPrefixType.Whip;

    public override void SetStaticDefaults()
    {
        Description = LocalizationManager.GetPrefixLocalization(this, "Payback", nameof(Description));
    }

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        string desc = Description.Format(PrefixBalance.PAYBACK_MIN_COINS, PrefixBalance.PAYBACK_MAX_COINS, (int)(PrefixBalance.PAYBACK_COPPER_MULT * 100),
            (int)(PrefixBalance.PAYBACK_SILVER_MULT * 100), (int)(PrefixBalance.PAYBACK_GOLD_MULT * 100), (int)(PrefixBalance.PAYBACK_PLATINUM_MULT * 100),
            (int)(PrefixBalance.PAYBACK_COIN_RETURN_CHANCE * 100));

        yield return new TooltipLine(Mod, "PaybackDescription", desc)
        {
            OverrideColor = Color.Gold
        };
    }
}