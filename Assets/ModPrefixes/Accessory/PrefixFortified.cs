using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Stats.Defense;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Accessory;

public class PrefixFortified : BaseEvolvedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ACCESSORY_REFORGING_MULTIPLIER;

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "newLine", Description.Format(PrefixBalance.FORTIFIED_INCREASED_DEFENSE * 100))
        {
            IsModifier = true
        };
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        StatPlayer statPlayer = player.GetModPlayer<StatPlayer>();
        statPlayer.AddStat(ModContent.GetInstance<DefenseMulStat>(), PrefixBalance.FORTIFIED_INCREASED_DEFENSE, StatSource.Accessory);
    }
}