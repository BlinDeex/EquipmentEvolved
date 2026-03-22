using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Summoner.Whips.Detonating;

public class PrefixDetonating : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;
    public override float ReforgeMultiplier => PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Whip;

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
    {
        useTimeMult *= PrefixBalance.DETONATING_USE_SPEED_MUL; 
    }

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        float delaySeconds = (float)Math.Round(PrefixBalance.DETONATING_AUTO_EXPLODE_TICKS / 60f, 1);
        int damagePercent = (int)(PrefixBalance.DETONATING_DAMAGE_MULTIPLIER * 100);
        
        string formattedDescription = Description.Format(delaySeconds, damagePercent);

        yield return new TooltipLine(Mod, "DetonatingDesc", formattedDescription)
        {
            IsModifier = true
        };
    }
}