using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Core;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using EquipmentEvolved.Assets.Stats.Defense;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Headwear.Reinforced;

public class PrefixReinforced : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Headwear;

    public LocalizedText NoSetBonus { get; private set; }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults(); // Gets the base Description safely
        NoSetBonus = LocalizationManager.GetSharedLocalizedText(LocalizationManager.NoArmorSetBonus);
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        int defenseAdded = (int)(item.defense * PrefixBalance.REINFORCED_DEFENSE_AMP);
        
        yield return new TooltipLine(Mod, "newLine", Description.Format(defenseAdded, MathF.Round(PrefixBalance.REINFORCED_DEFENSE_AMP * 100, 2)))
        {
            IsModifier = true
        };

        yield return new TooltipLine(Mod, "newLine2", NoSetBonus.Value)
        {
            IsModifier = true,
            IsModifierBad = true
        };
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        if (player.HasBuff<ArmorAbilityCooldownBuff>()) return;

        Item item = player.armor[0];
        int defenseAdded = (int)(item.defense * PrefixBalance.REINFORCED_DEFENSE_AMP);

        player.GetModPlayer<StatPlayer>().AddStat(ModContent.GetInstance<FlatDefenseStat>(), defenseAdded, StatSource.Headwear);
    }
}