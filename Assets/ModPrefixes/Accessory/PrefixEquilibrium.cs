using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPlayers;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Accessory;

public class PrefixEquilibrium : ModPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;

    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"Equilibrium", "DisplayName");

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ACCESSORY_REFORGING_MULTIPLIER;
    }

    public static LocalizedText FortifiedDesc { get; private set; }
    public static LocalizedText WarlordDesc { get; private set; }
    public static LocalizedText AerodynamicDesc { get; private set; }
    public static LocalizedText RevitalizingDesc { get; private set; }

    public override void SetStaticDefaults()
    {
        FortifiedDesc = LocalizationManager.GetPrefixLocalization(this,"Equilibrium", nameof(FortifiedDesc));
        WarlordDesc = LocalizationManager.GetPrefixLocalization(this,"Equilibrium", nameof(WarlordDesc));
        AerodynamicDesc = LocalizationManager.GetPrefixLocalization(this,"Equilibrium", nameof(AerodynamicDesc));
        RevitalizingDesc = LocalizationManager.GetPrefixLocalization(this,"Equilibrium", nameof(RevitalizingDesc));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        TooltipLine newLine =
            new TooltipLine(Mod, "newLine", FortifiedDesc.Format(PrefixBalance.EQUILIBRIUM_DEFENSE * 100))
            {
                IsModifier = true
            };
        TooltipLine newLine2 =
            new TooltipLine(Mod, "newLine2", WarlordDesc.Format(PrefixBalance.EQUILIBRIUM_MINIONS))
            {
                IsModifier = true
            };
        TooltipLine newLine3 =
            new TooltipLine(Mod, "newLine3",
                AerodynamicDesc.Format(PrefixBalance.EQUILIBRIUM_MOVEMENT_MULTIPLIER * 100,
                    PrefixBalance.EQUILIBRIUM_WING_TIME_TICKS))
            {
                IsModifier = true
            };
        TooltipLine newLine4 =
            new TooltipLine(Mod, "newLine4", RevitalizingDesc.Format(PrefixBalance.EQUILIBRIUM_REGENERATION / 2f))
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

        statPlayer.PickSpeedMul += PrefixBalance.AERODYNAMIC_MOVEMENT_MULTIPLIER;

        statPlayer.AdditionalMinions += PrefixBalance.EQUILIBRIUM_MINIONS;

        statPlayer.DefenseMul += PrefixBalance.EQUILIBRIUM_DEFENSE;

        statPlayer.MovementSpeedMul += PrefixBalance.EQUILIBRIUM_MOVEMENT_MULTIPLIER;
        statPlayer.WingTime += PrefixBalance.EQUILIBRIUM_WING_TIME_TICKS;
    }
}