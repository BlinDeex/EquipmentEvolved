using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.GiantSlayer;

public class PrefixGiantSlayer : BaseEvolvedPrefix, ISpecializedPrefix, IExperimentalPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;
    public override float ReforgeMultiplier => PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.RangedWeapon;

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "newLine", Description.Format(PrefixBalance.EQUALIZER_PERCENT_DAMAGE))
        {
            OverrideColor = Color.White
        };
    }
}