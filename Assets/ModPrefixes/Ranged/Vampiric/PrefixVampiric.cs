using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Vampiric;

public class PrefixVampiric : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;
    public override float ReforgeMultiplier => PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.RangedWeapon;

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
    {
        useTimeMult *= PrefixBalance.VAMPIRIC_FIRERATE;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        // Double check your original .Format() arguments here!
        yield return new TooltipLine(Mod, "newLine2", Description.Format(MathF.Round((1f - PrefixBalance.VAMPIRIC_FIRERATE) * 100)))
        {
            IsModifier = true
        };
    }
}