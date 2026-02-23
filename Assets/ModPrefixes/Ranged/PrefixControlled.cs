using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged;

public class PrefixControlled : ModPrefix, ISpecializedPrefix
{
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.RangedWeapon;
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult,
        ref float scaleMult,
        ref float shootSpeedMult, ref float manaMult, ref int critBonus)
    {
        useTimeMult *= PrefixBalance.CONTROLLED_FIRERATE;
    }
    
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"Controlled", "DisplayName");


    public static LocalizedText BurstFire { get; private set; }
    public static LocalizedText IncreasedVelocity { get; private set; }

    public override void SetStaticDefaults()
    {
        BurstFire = LocalizationManager.GetPrefixLocalization(this,"Controlled", nameof(BurstFire));
        IncreasedVelocity = LocalizationManager.GetPrefixLocalization(this,"Controlled", nameof(IncreasedVelocity));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        TooltipLine newLine = new TooltipLine(Mod, "newLine",
            BurstFire.Value)
        {
            IsModifier = true,
            IsModifierBad = false
        };

        TooltipLine newLine2 = new TooltipLine(Mod, "newLine2",
            IncreasedVelocity.Format((PrefixBalance.CONTROLLED_BULLET_VELOCITY - 1f) * 100))
        {
            IsModifier = true,
            IsModifierBad = false
        };


        yield return newLine2;
        yield return newLine;
    }

    public override bool CanRoll(Item item)
    {
        return item.autoReuse;
    }
}