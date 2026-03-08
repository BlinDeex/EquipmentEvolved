using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Ricochet;

public class PrefixRicochet : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Ranged;

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Ricochet", "DisplayName");

    public static LocalizedText Description { get; private set; }
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.RangedWeapon;

    public override void SetStaticDefaults()
    {
        Description = LocalizationManager.GetPrefixLocalization(this, "Ricochet", nameof(Description));
    }

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        int maxSplits = PrefixBalance.RICOCHET_MAX_SPLITS;
        int projCount = PrefixBalance.RICOCHET_PROJECTILES_PER_SPLIT;
        int damagePct = (int)(PrefixBalance.RICOCHET_DAMAGE_MULTIPLIER_PER_SPLIT * 100);

        string formattedDescription = Description.Format(maxSplits, projCount, damagePct);

        yield return new TooltipLine(Mod, "RicochetDescription", formattedDescription)
        {
            OverrideColor = Color.Orange
        };
    }
}