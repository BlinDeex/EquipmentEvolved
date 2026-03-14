using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Stats.MobilityUtility;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Accessory;

public class PrefixGreedy : BaseEvolvedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ACCESSORY_REFORGING_MULTIPLIER;

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        int dropInc = (int)((PrefixBalance.GREEDY_COIN_DROP_MULT - 1f) * 100);

        yield return new TooltipLine(Mod, "newLine", Description.Format(dropInc))
        {
            OverrideColor = Color.Gold,
            IsModifier = true
        };
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        StatPlayer statPlayer = player.GetModPlayer<StatPlayer>();
        float baseBonus = PrefixBalance.GREEDY_COIN_DROP_MULT - 1f;
        statPlayer.AddStat(ModContent.GetInstance<CoinDropMulStat>(), baseBonus, StatSource.Accessory);
    }
}