using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Accessory;

public class PrefixRisky : ModPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Risky", "DisplayName");

    public static LocalizedText DescDefense { get; private set; }
    public static LocalizedText DamageDesc { get; private set; }

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ACCESSORY_REFORGING_MULTIPLIER;
    }

    public override void SetStaticDefaults()
    {
        DescDefense = LocalizationManager.GetPrefixLocalization(this, "Risky", nameof(DescDefense));
        DamageDesc = LocalizationManager.GetSharedLocalizedText(LocalizationManager.XDamageAdded);
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        TooltipLine newLine = new(Mod, "newLine", DescDefense.Format(PrefixBalance.RISKY_DEFENSE_DECREASE * 100))
        {
            IsModifier = true,
            IsModifierBad = true
        };

        TooltipLine newLine2 = new(Mod, "newLine", DamageDesc.Format(Math.Round(PrefixBalance.RISKY_DAMAGE_INCREASE * 100, 2)))
        {
            IsModifier = true
        };

        yield return newLine;
        yield return newLine2;
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        if (!player.TryGetModPlayer(out StatPlayer statPlayer)) return;

        statPlayer.DamageMul += statPlayer.CalculateStatBonus(PrefixBalance.RISKY_DAMAGE_INCREASE, StatSource.AccessoryReforge);
        statPlayer.DefenseMul += statPlayer.CalculateStatBonus(PrefixBalance.RISKY_DEFENSE_DECREASE, StatSource.AccessoryReforge);
    }
}