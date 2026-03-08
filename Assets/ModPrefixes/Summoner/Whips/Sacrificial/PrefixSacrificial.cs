using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Summoner.Whips.Sacrificial;

public class PrefixSacrificial : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Sacrificial", "DisplayName");
    
    public static LocalizedText SacrificialDesc { get; private set; }
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Whip;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
    {
        useTimeMult *= PrefixBalance.CONTROLLED_FIRERATE;
    }

    public override void SetStaticDefaults()
    {
        SacrificialDesc = LocalizationManager.GetPrefixLocalization(this, "Sacrificial", nameof(SacrificialDesc));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        TooltipLine newLine = new(Mod, "newLine", SacrificialDesc.Value)
        {
            IsModifier = true
        };

        yield return newLine;
    }
}