using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Accessory;

public class PrefixBloodForged : ModPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "BloodForged", "DisplayName");

    public static LocalizedText MaxHealth { get; private set; }
    public static LocalizedText Defense { get; private set; }

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ACCESSORY_REFORGING_MULTIPLIER;
    }

    public override void SetStaticDefaults()
    {
        MaxHealth = LocalizationManager.GetPrefixLocalization(this, "BloodForged", nameof(MaxHealth));
        Defense = LocalizationManager.GetPrefixLocalization(this, "BloodForged", nameof(Defense));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        TooltipLine newLine = new(Mod, "newLine", Defense.Format(MathF.Round(PrefixBalance.BLOOD_FORGED_DEFENSE * 100f, 2)))
        {
            IsModifier = true
        };

        TooltipLine newLine2 = new(Mod, "newLine2", MaxHealth.Format(MathF.Round(-PrefixBalance.BLOOD_FORGED_MAX_HEALTH * 100f, 2)))
        {
            IsModifier = true,
            IsModifierBad = true
        };

        yield return newLine;
        yield return newLine2;
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        StatPlayer statPlayer = player.GetModPlayer<StatPlayer>();
        statPlayer.HealthCapMul *= 1 + statPlayer.CalculateStatBonus(PrefixBalance.BLOOD_FORGED_MAX_HEALTH, StatSource.AccessoryReforge);
        statPlayer.DefenseMul += statPlayer.CalculateStatBonus(PrefixBalance.BLOOD_FORGED_DEFENSE, StatSource.AccessoryReforge);
    }
}