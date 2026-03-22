using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Stats.MobilityUtility;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Accessory;

public class PrefixLucky : BaseEvolvedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ACCESSORY_REFORGING_MULTIPLIER;

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        int luckInc = (int)((PrefixBalance.LUCKY_CHARM_LUCK_MULT - 1f) * 100);

        yield return new TooltipLine(Mod, "LuckyDescription", Description.Format(luckInc))
        {
            IsModifier = true
        };
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        StatPlayer statPlayer = player.GetModPlayer<StatPlayer>();
        float baseBonus = MathF.Round(PrefixBalance.LUCKY_CHARM_LUCK_MULT - 1f, 2);
        statPlayer.AddStat(ModContent.GetInstance<CharmLuckStat>(), baseBonus, StatSource.Accessory);
    }
}