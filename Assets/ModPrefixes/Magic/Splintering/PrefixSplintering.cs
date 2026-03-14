using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Magic.Splintering;

public class PrefixSplintering : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;
    public override float ReforgeMultiplier => PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.MagicWeapon;

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "newLine", Description.Format(PrefixBalance.SPLINTERING_CHANCE * 100))
        {
            IsModifier = true
        };
    }
}