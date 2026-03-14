using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Magic.Overloaded;

public class PrefixOverloaded : BaseEvolvedPrefix, ISpecializedPrefix, IExperimentalPrefix // TODO add sound effect upon release
{
    public override PrefixCategory Category => PrefixCategory.Magic;
    public override float ReforgeMultiplier => PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.MagicWeapon;

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        int damageEfficiency = (int)(PrefixBalance.OVERLOADED_DAMAGE_EFFICIENCY * 100);

        yield return new TooltipLine(Mod, "OverloadedDescription", Description.Format(damageEfficiency))
        {
            OverrideColor = Color.Magenta
        };
    }
}