using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged;

public class PrefixChallenger : ModPrefix, ISpecializedPrefix
{
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.RangedWeapon;
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }

    public static LocalizedText Title { get; private set; }
    public static LocalizedText GreenOrb { get; private set; }
    public static LocalizedText BlueOrb { get; private set; }
    public static LocalizedText YellowOrb { get; private set; }
    public static LocalizedText RedOrb { get; private set; }

    public static LocalizedText Desc { get; private set; }

    public static LocalizedText Desc2 { get; private set; }
    
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"Challenger", "DisplayName");



    public override void SetStaticDefaults()
    {
        Title = LocalizationManager.GetPrefixLocalization(this,"Challenger", nameof(Title));
        GreenOrb = LocalizationManager.GetPrefixLocalization(this,"Challenger", nameof(GreenOrb));
        BlueOrb = LocalizationManager.GetPrefixLocalization(this,"Challenger", nameof(BlueOrb));
        YellowOrb = LocalizationManager.GetPrefixLocalization(this,"Challenger", nameof(YellowOrb));
        RedOrb = LocalizationManager.GetPrefixLocalization(this,"Challenger", nameof(RedOrb));
        Desc = LocalizationManager.GetPrefixLocalization(this,"Challenger", nameof(Desc));
        Desc2 = LocalizationManager.GetPrefixLocalization(this,"Challenger", nameof(Desc2));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        TooltipLine newLine = new TooltipLine(Mod, "newLine",
            Title.Value)
        {
            OverrideColor = Color.SandyBrown
        };

        TooltipLine newLine2 = new TooltipLine(Mod, "newLine2",
            GreenOrb.Format((int)(PrefixBalance.CHALLENGER_GREEN_ORB_DAMAGE * 100f)))
        {
            OverrideColor = Color.Green
        };

        TooltipLine newLine3 = new TooltipLine(Mod, "newLine3",
            BlueOrb.Format((int)(PrefixBalance.CHALLENGER_BLUE_ORB_VELOCITY * 100f),
                (int)(PrefixBalance.CHALLENGER_BLUE_ORB_CRIT * 100f)))
        {
            OverrideColor = Color.Blue
        };

        TooltipLine newLine4 = new TooltipLine(Mod, "newLine4",
            YellowOrb.Format(PrefixBalance.CHALLENGER_YELLOW_ORB_HEAL))
        {
            OverrideColor = Color.Yellow
        };

        TooltipLine newLine5 = new TooltipLine(Mod, "newLine5",
            RedOrb.Format(PrefixBalance.CHALLENGER_RED_ORB_DAMAGE))
        {
            OverrideColor = Color.Red
        };

        TooltipLine newLine6 = new TooltipLine(Mod, "newLine6",
            Desc.Value)
        {
            OverrideColor = Color.SandyBrown
        };

        TooltipLine newLine7 = new TooltipLine(Mod, "newLine7",
            Desc2.Format(PrefixBalance.CHALLENGER_ACTIVATION_SCORE_THRESHOLD,
                PrefixBalance.CHALLENGER_DEACTIVATION_SCORE_THRESHOLD))
        {
            OverrideColor = Color.SandyBrown
        };

        yield return newLine;
        yield return newLine2;
        yield return newLine3;
        yield return newLine4;
        yield return newLine5;
        yield return newLine6;
        yield return newLine7;
    }
}