using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.Perceptive;

public class PrefixPerceptive : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;

    public static LocalizedText Desc { get; private set; }

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Perceptive", "DisplayName");

    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.MeleeWeapon;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }


    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this, "Perceptive", nameof(Desc));
    }

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
    {
        critBonus += PrefixBalance.PERCEPTIVE_CRIT;
        damageMult *= PrefixBalance.PERCEPTIVE_CRIT_DAMAGE;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        TooltipLine newLine = new(Mod, "newLine", Desc.Format(Math.Clamp(Main.LocalPlayer.GetWeaponCrit(item), 0, 200) - 100))
        {
            OverrideColor = Color.YellowGreen
        };

        yield return newLine;
    }
}