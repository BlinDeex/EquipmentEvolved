using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Delayed;

public class PrefixDelayed : BaseEvolvedPrefix, ISpecializedPrefix, IExperimentalPrefix
{
    public override PrefixCategory Category => PrefixCategory.Ranged;
    public override float ReforgeMultiplier => PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.RangedWeapon;

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        float pulseSeconds = PrefixBalance.DELAYED_PULSE_TIMER / 60f;
        int laserDamagePct = (int)(PrefixBalance.DELAYED_LASER_DAMAGE_MULT * 100);
        
        string formattedDescription = Description.Format(pulseSeconds, laserDamagePct);
        
        yield return new TooltipLine(Mod, "DelayedDescription", formattedDescription)
        {
            IsModifier = true
        };
    }
}