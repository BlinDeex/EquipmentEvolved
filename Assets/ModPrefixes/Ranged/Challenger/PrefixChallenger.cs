using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Challenger;

public class PrefixChallenger : BaseEvolvedPrefix, ISpecializedPrefix, IExperimentalPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;
    public override float ReforgeMultiplier => PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.RangedWeapon;

    public LocalizedText Title { get; private set; }
    public LocalizedText GreenOrb { get; private set; }
    public LocalizedText BlueOrb { get; private set; }
    public LocalizedText YellowOrb { get; private set; }
    public LocalizedText RedOrb { get; private set; }
    public LocalizedText Desc2 { get; private set; }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults(); // Automatically maps your old "Desc" to Description!
        Title = GetLoc(nameof(Title));
        GreenOrb = GetLoc(nameof(GreenOrb));
        BlueOrb = GetLoc(nameof(BlueOrb));
        YellowOrb = GetLoc(nameof(YellowOrb));
        RedOrb = GetLoc(nameof(RedOrb));
        Desc2 = GetLoc(nameof(Desc2));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "newLine", Title.Value) { OverrideColor = Color.SandyBrown };
        yield return new TooltipLine(Mod, "newLine2", GreenOrb.Format((int)(PrefixBalance.CHALLENGER_GREEN_ORB_DAMAGE * 100f))) { OverrideColor = Color.Green };
        yield return new TooltipLine(Mod, "newLine3", BlueOrb.Format((int)(PrefixBalance.CHALLENGER_BLUE_ORB_VELOCITY * 100f), (int)(PrefixBalance.CHALLENGER_BLUE_ORB_CRIT * 100f))) { OverrideColor = Color.Blue };
        yield return new TooltipLine(Mod, "newLine4", YellowOrb.Format(PrefixBalance.CHALLENGER_YELLOW_ORB_HEAL)) { OverrideColor = Color.Yellow };
        yield return new TooltipLine(Mod, "newLine5", RedOrb.Format(PrefixBalance.CHALLENGER_RED_ORB_DAMAGE)) { OverrideColor = Color.Red };
        
        yield return new TooltipLine(Mod, "newLine6", Description.Value) { OverrideColor = Color.SandyBrown };
        
        yield return new TooltipLine(Mod, "newLine7", Desc2.Format(PrefixBalance.CHALLENGER_ACTIVATION_SCORE_THRESHOLD, PrefixBalance.CHALLENGER_DEACTIVATION_SCORE_THRESHOLD)) { OverrideColor = Color.SandyBrown };
    }
}