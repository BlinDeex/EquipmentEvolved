using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;

namespace EquipmentEvolved.Assets.ModPrefixes.Summoner.MinionWeapons.Frenzied;

public class PrefixFrenzied : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;

    public static LocalizedText Desc { get; private set; }

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Frenzied", "DisplayName");

    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.MinionWeapon;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }


    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this, "Frenzied", nameof(Desc));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        TooltipLine newLine = new(Mod, "newLine", Desc.Format(PrefixBalance.EQUALIZER_PERCENT_DAMAGE))
        {
            OverrideColor = Color.WhiteSmoke
        };

        yield return newLine;
    }
}