using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Thaumic;

public class PrefixThaumic : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Ranged;

    public static LocalizedText Desc { get; private set; }

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Thaumic", "DisplayName");

    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.RangedWeapon;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }

    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this, "Thaumic", nameof(Desc));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        float baseDurationSeconds = PrefixBalance.THAUMIC_BASE_DURATION_TICKS / 60f;
        float extensionSeconds = PrefixBalance.THAUMIC_EXTENSION_TICKS / 60f;
        
        yield return new TooltipLine(Mod, "ThaumicDesc", Desc.Format(baseDurationSeconds, extensionSeconds))
        {
            IsModifier = true,
            IsModifierBad = false
        };
    }
}