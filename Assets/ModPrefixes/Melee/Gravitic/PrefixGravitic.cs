using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.Gravitic;

public class PrefixGravitic : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Melee;
    public override float ReforgeMultiplier => PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Any; 

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        float calculatedRadius = PrefixBalance.GRAVITIC_BASE_PULL_RADIUS + (item.useTime * PrefixBalance.GRAVITIC_RADIUS_PER_USE_TIME);
        
        float combinedScale = item.scale * Main.LocalPlayer.GetAdjustedItemScale(item);
        float currentRadius = Math.Clamp(calculatedRadius * combinedScale, 0f, PrefixBalance.GRAVITIC_MAX_PULL_RADIUS);
        
        float currentPullSpeed = PrefixBalance.GRAVITIC_BASE_PULL_SPEED * (float)Math.Pow(PrefixBalance.GRAVITIC_SPEED_EXPONENT, item.useTime);
        currentPullSpeed = Math.Clamp(currentPullSpeed, 0f, PrefixBalance.GRAVITIC_MAX_PULL_SPEED);

        TooltipLine descLine = new(Mod, "GraviticDesc", Description.Format(MathF.Round(currentRadius), MathF.Round(currentPullSpeed, 1)))
        {
            IsModifier = true
        };

        yield return descLine;
    }
}