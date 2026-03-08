using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Magic.Endless;

public class PrefixEndless : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Endless", "DisplayName");

    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.MagicWeapon;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
    {
        manaMult *= PrefixBalance.ENDLESS_MANA_USAGE;
        damageMult *= PrefixBalance.ENDLESS_DAMAGE;
    }
}