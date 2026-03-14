using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Thaumic;

public class PrefixThaumic : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Ranged;
    public override float ReforgeMultiplier => PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.RangedWeapon;

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        float baseDurationSeconds = PrefixBalance.THAUMIC_BASE_DURATION_TICKS / 60f;
        float extensionSeconds = PrefixBalance.THAUMIC_EXTENSION_TICKS / 60f;
        
        yield return new TooltipLine(Mod, "ThaumicDesc", Description.Format(baseDurationSeconds, extensionSeconds))
        {
            IsModifier = true
        };
    }
}