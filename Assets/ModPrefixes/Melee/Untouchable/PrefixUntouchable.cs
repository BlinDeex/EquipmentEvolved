using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.Untouchable;

public class PrefixUntouchable : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;
    public override float ReforgeMultiplier => PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.MeleeWeapon | SpecializedPrefixType.Whip;

    public LocalizedText IncreasedDamageTaken { get; private set; }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults(); // BaseEvolvedPrefix handles "Description" for you!
        IncreasedDamageTaken = GetLoc(nameof(IncreasedDamageTaken));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "newLine", Description.Value)
        {
            IsModifier = true
        };

        yield return new TooltipLine(Mod, "newLine2", IncreasedDamageTaken.Value)
        {
            IsModifier = true,
            IsModifierBad = true
        };

        if (!Main.LocalPlayer.TryGetModPlayer(out UntouchableModPlayer prefixPlayer)) yield break;

        yield return new TooltipLine(Mod, "newLine3", LocalizationManager.GetSharedLocalizedText(LocalizationManager.XDamageAdded).Format(Math.Round(prefixPlayer.UntouchableDamageIncrease * 100, 2)))
        {
            IsModifier = true
        };
    }
}