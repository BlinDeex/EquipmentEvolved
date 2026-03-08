using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Accessory;

public class PrefixEquilibrium : ModPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Equilibrium", "DisplayName");

    public static LocalizedText FortifiedDesc { get; private set; }
    public static LocalizedText WarlordDesc { get; private set; }
    public static LocalizedText AerodynamicDesc { get; private set; }
    public static LocalizedText RevitalizingDesc { get; private set; }

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ACCESSORY_REFORGING_MULTIPLIER;
    }

    public override void SetStaticDefaults()
    {
        FortifiedDesc = LocalizationManager.GetPrefixLocalization(this, "Equilibrium", nameof(FortifiedDesc));
        WarlordDesc = LocalizationManager.GetPrefixLocalization(this, "Equilibrium", nameof(WarlordDesc));
        AerodynamicDesc = LocalizationManager.GetPrefixLocalization(this, "Equilibrium", nameof(AerodynamicDesc));
        RevitalizingDesc = LocalizationManager.GetPrefixLocalization(this, "Equilibrium", nameof(RevitalizingDesc));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        TooltipLine newLine = new(Mod, "newLine", FortifiedDesc.Format(PrefixBalance.EQUILIBRIUM_DEFENSE * 100))
        {
            IsModifier = true
        };
        TooltipLine newLine2 = new(Mod, "newLine2", WarlordDesc.Format(PrefixBalance.EQUILIBRIUM_MINIONS))
        {
            IsModifier = true
        };
        TooltipLine newLine3 = new(Mod, "newLine3", AerodynamicDesc.Format(PrefixBalance.EQUILIBRIUM_MOVEMENT_MULTIPLIER * 100, PrefixBalance.EQUILIBRIUM_WING_TIME_TICKS))
        {
            IsModifier = true
        };
        TooltipLine newLine4 = new(Mod, "newLine4", RevitalizingDesc.Format(PrefixBalance.EQUILIBRIUM_REGENERATION / 2f))
        {
            IsModifier = true
        };

        yield return newLine;
        yield return newLine2;
        yield return newLine3;
        yield return newLine4;
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        if (!player.TryGetModPlayer(out StatPlayer statPlayer)) return;

        statPlayer.PickSpeedMul += statPlayer.CalculateStatBonus(PrefixBalance.AERODYNAMIC_MOVEMENT_MULTIPLIER, StatSource.AccessoryReforge);

        statPlayer.AdditionalMinions += statPlayer.CalculateStatBonus(PrefixBalance.EQUILIBRIUM_MINIONS, StatSource.AccessoryReforge);

        statPlayer.DefenseMul += statPlayer.CalculateStatBonus(PrefixBalance.EQUILIBRIUM_DEFENSE, StatSource.AccessoryReforge);

        statPlayer.MovementSpeedMul += statPlayer.CalculateStatBonus(PrefixBalance.EQUILIBRIUM_MOVEMENT_MULTIPLIER, StatSource.AccessoryReforge);

        statPlayer.WingTime += (int)statPlayer.CalculateStatBonus(PrefixBalance.EQUILIBRIUM_WING_TIME_TICKS, StatSource.AccessoryReforge);
    }
}