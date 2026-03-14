using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Pickaxe.Geological;

public class PrefixGeological : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Melee;
    public override float ReforgeMultiplier => PrefixBalance.TOOL_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Pickaxe;

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        int chance = (int)(PrefixBalance.GEOLOGICAL_GEM_DROP_CHANCE * 100);

        yield return new TooltipLine(Mod, "GeologicalDescription", Description.Format(chance))
        {
            OverrideColor = Color.LightGoldenrodYellow
        };
    }
}