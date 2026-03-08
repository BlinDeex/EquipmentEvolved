using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Magic.Chaotic;

public class PrefixChaotic : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Chaotic", "DisplayName");

    public static LocalizedText Desc { get; private set; }
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.MagicWeapon;

    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this, "Chaotic", nameof(Desc));
    }

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        TooltipLine newLine = new(Mod, "newLine", Desc.Value)
        {
            OverrideColor = Main.DiscoColor
        };

        yield return newLine;
    }
}