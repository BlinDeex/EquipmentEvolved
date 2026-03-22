using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.Perceptive;

public class PrefixPerceptive : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;
    public override float ReforgeMultiplier => PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.MeleeWeapon;

    public LocalizedText WarningText { get; private set; }

    protected override void OnSetStaticDefaults()
    {
         
        WarningText = GetLoc("Warning");
    }

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
    {
        critBonus += PrefixBalance.PERCEPTIVE_CRIT;
        damageMult *= PrefixBalance.PERCEPTIVE_CRIT_DAMAGE;
    }

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        int totalCrit = Main.LocalPlayer.GetWeaponCrit(item);

        if (totalCrit <= 100)
        {
            yield return new TooltipLine(Mod, "PerceptiveWarning", WarningText.Value)
            {
                IsModifier = true,
                IsModifierBad = true
            };
        }
        else
        {
            int overCritCap = PrefixBalance.PERCEPTIVE_MAX_TIER * 100;
            int overCritChance = Math.Clamp(totalCrit - 100, 0, overCritCap);
            
            float maxMultiplier = 1f + 1f + ((PrefixBalance.PERCEPTIVE_MAX_TIER - 1) * 0.5f);
            
            yield return new TooltipLine(Mod, "PerceptiveDesc", Description.Format(overCritChance, PrefixBalance.PERCEPTIVE_MAX_TIER, maxMultiplier))
            {
                IsModifier = true,
                IsModifierBad = false
            };
        }
    }
}