using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using EquipmentEvolved.Assets.Stats.Defense;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Accessory;

public class PrefixBloodForged : BaseEvolvedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ACCESSORY_REFORGING_MULTIPLIER;

    // These are no longer static!
    public LocalizedText MaxHealth { get; private set; }
    public LocalizedText Defense { get; private set; }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults(); // Gets the normal description setup safely
        MaxHealth = GetLoc(nameof(MaxHealth));
        Defense = GetLoc(nameof(Defense));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "newLine", Defense.Format(MathF.Round(PrefixBalance.BLOOD_FORGED_DEFENSE * 100f, 2)))
        { 
            IsModifier = true 
        };

        yield return new TooltipLine(Mod, "newLine2", MaxHealth.Format(MathF.Round(-PrefixBalance.BLOOD_FORGED_MAX_HEALTH * 100f, 2)))
        { 
            IsModifier = true, 
            IsModifierBad = true 
        };
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        StatPlayer statPlayer = player.GetModPlayer<StatPlayer>();
        statPlayer.AddStat(ModContent.GetInstance<HealthCapMulStat>(), PrefixBalance.BLOOD_FORGED_MAX_HEALTH, StatSource.Accessory);
        statPlayer.AddStat(ModContent.GetInstance<DefenseMulStat>(), PrefixBalance.BLOOD_FORGED_DEFENSE, StatSource.Accessory);
    }
}