using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Ricochet;

public class PrefixRicochet : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Ranged;
    public override float ReforgeMultiplier => PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.RangedWeapon;

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        int maxSplits = PrefixBalance.RICOCHET_MAX_SPLITS;
        int projCount = PrefixBalance.RICOCHET_PROJECTILES_PER_SPLIT;
        int damagePct = (int)(PrefixBalance.RICOCHET_DAMAGE_MULTIPLIER_PER_SPLIT * 100);

        string formattedDescription = Description.Format(maxSplits, projCount, damagePct);

        yield return new TooltipLine(Mod, "newLine", formattedDescription)
        {
            IsModifier = true
        };
    }
}