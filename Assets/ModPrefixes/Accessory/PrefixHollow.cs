using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Accessory;

public class PrefixHollow : ModPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;

    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this, null, "DisplayName");
    public static LocalizedText Description { get; private set; }

    public override void SetStaticDefaults()
    {
        Description = LocalizationManager.GetPrefixLocalization(this, null, nameof(Description));
    }

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ACCESSORY_REFORGING_MULTIPLIER;
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        StatPlayer statPlayer = player.GetModPlayer<StatPlayer>();

        float healthBonus = PrefixBalance.HOLLOW_MAX_HEALTH_MULT - 1f;
        statPlayer.MaxHealthMul += statPlayer.CalculateStatBonus(healthBonus, StatSource.AccessoryReforge);

        float baseMobility = PrefixBalance.HOLLOW_MOBILITY_MULT - 1f;
        float finalMobility = statPlayer.CalculateStatBonus(baseMobility, StatSource.AccessoryReforge);

        statPlayer.MovementSpeedMul += finalMobility;
        statPlayer.WingHorizontalAcc += finalMobility;
        statPlayer.WingHorizontalSpeed += finalMobility;
        statPlayer.WingVerticalAcc += finalMobility;
        statPlayer.WingVerticalSpeed += finalMobility;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        int healthDec = (int)((1f - PrefixBalance.HOLLOW_MAX_HEALTH_MULT) * 100);
        int mobInc = (int)((PrefixBalance.HOLLOW_MOBILITY_MULT - 1f) * 100);

        string formattedDescription = Description.Format(healthDec, mobInc);

        yield return new TooltipLine(Mod, "HollowDescription", formattedDescription)
        {
            OverrideColor = Color.GhostWhite
        };
    }
}