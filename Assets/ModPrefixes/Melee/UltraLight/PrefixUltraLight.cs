using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.UltraLight;

public class PrefixUltraLight : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;
    public override float ReforgeMultiplier => PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.MeleeWeapon | SpecializedPrefixType.Whip;

    public LocalizedText AutoReuse { get; private set; }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults(); // Safely sets up standard Description if you ever add one!
        AutoReuse = GetLoc(nameof(AutoReuse));
    }

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
    {
        scaleMult *= PrefixBalance.ULTRA_LIGHT_SIZE;
        useTimeMult *= PrefixBalance.ULTRA_LIGHT_USE;
        damageMult *= PrefixBalance.ULTRA_LIGHT_DAMAGE;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "newLine", AutoReuse.Value)
        {
            IsModifier = true,
            IsModifierBad = false
        };
    }
}