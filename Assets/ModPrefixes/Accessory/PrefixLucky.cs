using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Accessory;

public class PrefixLucky : ModPrefix
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
        float baseBonus = PrefixBalance.LUCKY_CHARM_LUCK_MULT - 1f;
        statPlayer.CharmLuckMul += statPlayer.CalculateStatBonus(baseBonus, StatSource.AccessoryReforge);
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        int luckInc = (int)((PrefixBalance.LUCKY_CHARM_LUCK_MULT - 1f) * 100);

        yield return new TooltipLine(Mod, "LuckyDescription", Description.Format(luckInc))
        {
            OverrideColor = Color.Gold
        };
    }
}