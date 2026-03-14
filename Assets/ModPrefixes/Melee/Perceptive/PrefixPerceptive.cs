using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.Perceptive;

public class PrefixPerceptive : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;
    public override float ReforgeMultiplier => PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.MeleeWeapon;

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
    {
        critBonus += PrefixBalance.PERCEPTIVE_CRIT;
        damageMult *= PrefixBalance.PERCEPTIVE_CRIT_DAMAGE;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "newLine", Description.Format(Math.Clamp(Main.LocalPlayer.GetWeaponCrit(item), 0, 200) - 100))
        {
            OverrideColor = Color.YellowGreen
        };
    }
}