using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Stats.Combat;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Accessory;

public class PrefixZealous : BaseEvolvedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ACCESSORY_REFORGING_MULTIPLIER;

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        int speedInc = (int)((PrefixBalance.ZEALOUS_ATTACK_SPEED_MULT - 1f) * 100);

        yield return new TooltipLine(Mod, "ZealousDescription", Description.Format(speedInc))
        {
            IsModifier = true
        };
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        StatPlayer statPlayer = player.GetModPlayer<StatPlayer>();
        float baseBonus = PrefixBalance.ZEALOUS_ATTACK_SPEED_MULT - 1f;
        statPlayer.AddStat(ModContent.GetInstance<UseSpeedStat>(), baseBonus, StatSource.Accessory);
    }
}