using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Accessory;

public class PrefixGreedy : ModPrefix
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
        float baseBonus = PrefixBalance.GREEDY_COIN_DROP_MULT - 1f;
        statPlayer.CoinDropMul += statPlayer.CalculateStatBonus(baseBonus, StatSource.AccessoryReforge);
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        int dropInc = (int)((PrefixBalance.GREEDY_COIN_DROP_MULT - 1f) * 100);

        yield return new TooltipLine(Mod, "GreedyDescription", Description.Format(dropInc))
        {
            OverrideColor = Color.Gold
        };
    }
}