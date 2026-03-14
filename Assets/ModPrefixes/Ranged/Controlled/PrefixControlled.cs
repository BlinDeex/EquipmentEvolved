using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Controlled;

public class PrefixControlled : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;
    public override float ReforgeMultiplier => PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.RangedWeapon;

    public LocalizedText BurstFire { get; private set; }
    public LocalizedText IncreasedVelocity { get; private set; }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults(); // Gets the standard Description
        BurstFire = GetLoc(nameof(BurstFire));
        IncreasedVelocity = GetLoc(nameof(IncreasedVelocity));
    }

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
    {
        useTimeMult *= PrefixBalance.CONTROLLED_FIRERATE;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "newLine2", IncreasedVelocity.Format((PrefixBalance.CONTROLLED_BULLET_VELOCITY - 1f) * 100))
        {
            IsModifier = true,
            IsModifierBad = false
        };

        yield return new TooltipLine(Mod, "newLine", BurstFire.Value)
        {
            IsModifier = true,
            IsModifierBad = false
        };
    }

    public override bool CanRoll(Item item)
    {
        return item.autoReuse;
    }
}   