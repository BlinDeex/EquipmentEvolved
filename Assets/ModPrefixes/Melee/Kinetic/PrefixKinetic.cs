using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.Kinetic;

public class PrefixKinetic : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Kinetic", "DisplayName");

    public static LocalizedText Description { get; private set; }
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.MeleeWeapon;

    public override void SetStaticDefaults()
    {
        Description = LocalizationManager.GetPrefixLocalization(this, "Kinetic", nameof(Description));
    }

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
    {
        damageMult -= PrefixBalance.KINETIC_DAMAGE_NERF;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "KineticDescription", Description.Value)
        {
            OverrideColor = Color.LightSkyBlue
        };
    }
}