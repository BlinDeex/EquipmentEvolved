using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Stats.MobilityUtility;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Accessory;

public class PrefixLucky : BaseEvolvedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ACCESSORY_REFORGING_MULTIPLIER;

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        int luckInc = (int)((PrefixBalance.LUCKY_CHARM_LUCK_MULT - 1f) * 100);

        yield return new TooltipLine(Mod, "LuckyDescription", Description.Format(luckInc))
        {
            OverrideColor = Color.Gold
        };
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        StatPlayer statPlayer = player.GetModPlayer<StatPlayer>();
        float baseBonus = PrefixBalance.LUCKY_CHARM_LUCK_MULT - 1f;
        statPlayer.AddStat(ModContent.GetInstance<CharmLuckStat>(), baseBonus, StatSource.Accessory);
    }
}