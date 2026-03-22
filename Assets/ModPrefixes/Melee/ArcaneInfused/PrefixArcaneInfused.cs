using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.ArcaneInfused;

public class PrefixArcaneInfused : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;
    public override float ReforgeMultiplier => PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.MeleeWeapon | SpecializedPrefixType.Whip;

    public LocalizedText ManaPerSwing { get; private set; }
    public LocalizedText ManaSicknessWorks { get; private set; }

    protected override void OnSetStaticDefaults()
    {
         
        ManaPerSwing = GetLoc(nameof(ManaPerSwing));
        ManaSicknessWorks = GetLoc(nameof(ManaSicknessWorks));
    }

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
    {
        damageMult *= PrefixBalance.ARCANE_INFUSED_DAMAGE;
    }

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "manaLine", ManaPerSwing.Format(PrefixBalance.ARCANE_INFUSED_MANA_PER_SWING))
        {
            IsModifier = true,
            IsModifierBad = true
        };

        yield return new TooltipLine(Mod, "manaLine2", ManaSicknessWorks.Value)
        {
            IsModifier = true,
            IsModifierBad = true
        };
    }
}