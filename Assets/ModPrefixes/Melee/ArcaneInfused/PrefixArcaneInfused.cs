using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.ArcaneInfused;

public class PrefixArcaneInfused : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;

    public static LocalizedText ManaPerSwing { get; private set; }
    public static LocalizedText ManaSicknessWorks { get; private set; }

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "ArcaneInfused", "DisplayName");

    public SpecializedPrefixType SpecializedPrefixType =>
        SpecializedPrefixType.MeleeWeapon | SpecializedPrefixType.Whip;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
    {
        damageMult *= PrefixBalance.ARCANE_INFUSED_DAMAGE;
    }

    public override void SetStaticDefaults()
    {
        ManaPerSwing = LocalizationManager.GetPrefixLocalization(this, "ArcaneInfused", nameof(ManaPerSwing));
        ManaSicknessWorks = LocalizationManager.GetPrefixLocalization(this, "ArcaneInfused", nameof(ManaSicknessWorks));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        TooltipLine manaLine = new(Mod, "manaLine", ManaPerSwing.Format(PrefixBalance.ARCANE_INFUSED_MANA_PER_SWING))
        {
            IsModifier = true,
            IsModifierBad = true
        };

        TooltipLine manaLine2 = new(Mod, "manaLine2", ManaSicknessWorks.Value)
        {
            IsModifier = true,
            IsModifierBad = true
        };

        yield return manaLine;
        yield return manaLine2;
    }
}