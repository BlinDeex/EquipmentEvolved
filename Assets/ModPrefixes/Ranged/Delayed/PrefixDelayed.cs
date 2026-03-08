using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Delayed;

public class PrefixDelayed : ModPrefix, ISpecializedPrefix, IExperimentalPrefix
{
    public override PrefixCategory Category => PrefixCategory.Ranged;

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Delayed", "DisplayName");

    public static LocalizedText Description { get; private set; }
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.RangedWeapon;

    public override void SetStaticDefaults()
    {
        Description = LocalizationManager.GetPrefixLocalization(this, "Delayed", nameof(Description));
    }

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        float pulseSeconds = PrefixBalance.DELAYED_PULSE_TIMER / 60f;
        int laserDamagePct = (int)(PrefixBalance.DELAYED_LASER_DAMAGE_MULT * 100);
        
        string formattedDescription = Description.Format(pulseSeconds, laserDamagePct);
        
        yield return new TooltipLine(Mod, "DelayedDescription", formattedDescription)
        {
            OverrideColor = Color.Cyan
        };
    }
}