using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Stats.Combat;
using EquipmentEvolved.Assets.Stats.Defense;
using EquipmentEvolved.Assets.Stats.MobilityUtility;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Accessory;

public class PrefixEquilibrium : BaseEvolvedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ACCESSORY_REFORGING_MULTIPLIER;
    public LocalizedText FortifiedDesc { get; private set; }
    public LocalizedText WarlordDesc { get; private set; }
    public LocalizedText AerodynamicDesc { get; private set; }
    public LocalizedText RevitalizingDesc { get; private set; }

    protected override void OnSetStaticDefaults()
    {
        
        FortifiedDesc = GetLoc(nameof(FortifiedDesc));
        WarlordDesc = GetLoc(nameof(WarlordDesc));
        AerodynamicDesc = GetLoc(nameof(AerodynamicDesc));
        RevitalizingDesc = GetLoc(nameof(RevitalizingDesc));
    }

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "newLine", FortifiedDesc.Format(PrefixBalance.EQUILIBRIUM_DEFENSE * 100)) { IsModifier = true };
        yield return new TooltipLine(Mod, "newLine2", WarlordDesc.Format(PrefixBalance.EQUILIBRIUM_MINIONS)) { IsModifier = true };
        yield return new TooltipLine(Mod, "newLine3", AerodynamicDesc.Format(PrefixBalance.EQUILIBRIUM_MOVEMENT_MULTIPLIER * 100, PrefixBalance.EQUILIBRIUM_WING_TIME_TICKS)) { IsModifier = true };
        yield return new TooltipLine(Mod, "newLine4", RevitalizingDesc.Format(PrefixBalance.EQUILIBRIUM_REGENERATION / 2f)) { IsModifier = true };
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        StatPlayer statPlayer = player.GetModPlayer<StatPlayer>();

        statPlayer.AddStat(ModContent.GetInstance<PickSpeedStat>(), PrefixBalance.AERODYNAMIC_MOVEMENT_MULTIPLIER, StatSource.Accessory);
        statPlayer.AddStat(ModContent.GetInstance<MinionsStat>(), PrefixBalance.EQUILIBRIUM_MINIONS, StatSource.Accessory);
        statPlayer.AddStat(ModContent.GetInstance<DefenseMulStat>(), PrefixBalance.EQUILIBRIUM_DEFENSE, StatSource.Accessory);
        statPlayer.AddStat(ModContent.GetInstance<MoveSpeedStat>(), PrefixBalance.EQUILIBRIUM_MOVEMENT_MULTIPLIER, StatSource.Accessory);
        statPlayer.AddStat(ModContent.GetInstance<WingTimeStat>(), PrefixBalance.EQUILIBRIUM_WING_TIME_TICKS, StatSource.Accessory);
    }
}