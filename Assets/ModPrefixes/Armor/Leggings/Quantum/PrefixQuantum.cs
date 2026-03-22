using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Leggings.Quantum;

public class PrefixQuantum : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Leggings;

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "QuantumDesc", Description.Format(MathF.Round(PrefixBalance.QUANTUM_AFTERIMAGE_DEFENSE_MULT * 100, 2)))
        {
            IsModifier = true,
        };
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        player.GetModPlayer<QuantumModPlayer>().HasQuantumLeggings = true;
    }
}