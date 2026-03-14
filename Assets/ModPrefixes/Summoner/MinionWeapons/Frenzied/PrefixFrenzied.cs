using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;

namespace EquipmentEvolved.Assets.ModPrefixes.Summoner.MinionWeapons.Frenzied;

public class PrefixFrenzied : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;
    public override float ReforgeMultiplier => PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.MinionWeapon;

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "newLine", Description.Format(PrefixBalance.EQUALIZER_PERCENT_DAMAGE))
        {
            OverrideColor = Color.WhiteSmoke
        };
    }
}