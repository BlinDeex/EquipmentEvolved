using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Headwear.Phasing;

public class PrefixPhasing : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Headwear;

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        // Convert the raw ticks into easily readable seconds for the UI
        float dashSeconds = MathF.Round(PrefixBalance.PHASING_DASH_IFRAMES / 60f, 1);
        float tpSeconds = MathF.Round(PrefixBalance.PHASING_TELEPORT_IFRAMES / 60f, 1);

        TooltipLine descLine = new(Mod, "PhasingDesc", Description.Format(dashSeconds, tpSeconds))
        {
            IsModifier = true
        };

        yield return descLine;
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        player.GetModPlayer<PhasingModPlayer>().HasPhasing = true;
    }
}