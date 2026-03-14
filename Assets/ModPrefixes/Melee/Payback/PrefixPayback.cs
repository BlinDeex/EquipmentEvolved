using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.Payback;

public class PrefixPayback : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;
    public override float ReforgeMultiplier => PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.MeleeWeapon | SpecializedPrefixType.Whip;

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