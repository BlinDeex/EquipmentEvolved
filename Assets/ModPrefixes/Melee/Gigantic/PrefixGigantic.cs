using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.Gigantic;

public class PrefixGigantic : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Gigantic", "DisplayName");

    public SpecializedPrefixType SpecializedPrefixType =>
        SpecializedPrefixType.MeleeWeapon | SpecializedPrefixType.Whip;


    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
    {
        scaleMult *= PrefixBalance.GIGANTIC_SIZE;
        useTimeMult *= PrefixBalance.GIGANTIC_USE;
    }
}