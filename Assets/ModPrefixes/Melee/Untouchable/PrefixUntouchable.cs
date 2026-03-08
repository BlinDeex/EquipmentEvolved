using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.Untouchable;

public class PrefixUntouchable : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;

    public static LocalizedText Desc { get; private set; }
    public static LocalizedText IncreasedDamageTaken { get; private set; }

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Untouchable", "DisplayName");

    public SpecializedPrefixType SpecializedPrefixType =>
        SpecializedPrefixType.MeleeWeapon | SpecializedPrefixType.Whip;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }


    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this, "Untouchable", nameof(Desc));
        IncreasedDamageTaken = LocalizationManager.GetPrefixLocalization(this, "Untouchable", nameof(IncreasedDamageTaken));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        TooltipLine newLine = new(Mod, "newLine", Desc.Value)
        {
            IsModifier = true
        };

        TooltipLine newLine2 = new(Mod, "newLine", IncreasedDamageTaken.Value)
        {
            IsModifier = true,
            IsModifierBad = true
        };

        yield return newLine;
        yield return newLine2;

        if (!Main.LocalPlayer.TryGetModPlayer(out UntouchableModPlayer prefixPlayer)) yield break;

        TooltipLine newLine3 = new(Mod, "newLine2", LocalizationManager.GetSharedLocalizedText(LocalizationManager.XDamageAdded).Format(Math.Round(prefixPlayer.UntouchableDamageIncrease * 100, 2)))
        {
            IsModifier = true
        };


        yield return newLine3;
    }
}