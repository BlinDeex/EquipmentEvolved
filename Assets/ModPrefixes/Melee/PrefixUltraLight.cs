using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee;

public class PrefixUltraLight : ModPrefix, ISpecializedPrefix
{
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.MeleeWeapon;
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }

    public static LocalizedText AutoReuse { get; private set; }
    
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"UltraLight", "DisplayName");


    public override void SetStaticDefaults()
    {
        AutoReuse = LocalizationManager.GetPrefixLocalization(this,"UltraLight", nameof(AutoReuse));
    }

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult,
        ref float scaleMult,
        ref float shootSpeedMult, ref float manaMult, ref int critBonus)
    {
        scaleMult *= PrefixBalance.ULTRA_LIGHT_SIZE;
        useTimeMult *= PrefixBalance.ULTRA_LIGHT_USE;
        damageMult *= PrefixBalance.ULTRA_LIGHT_DAMAGE;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        TooltipLine newLine = new TooltipLine(Mod, "newLine",
            AutoReuse.Value)
        {
            IsModifier = true,
            IsModifierBad = false
        };

        yield return newLine;
    }
}