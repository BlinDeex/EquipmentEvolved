using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Core;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using EquipmentEvolved.Assets.Stats.Defense;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Headwear.Reinforced;

public class PrefixReinforced : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Headwear;

    

    protected override void OnSetStaticDefaults()
    {
        
        
        
        DefenseTooltipGlobalItem.DefenseModifiers.Add((item, _) =>
        {
            if (item.HasPrefix(ModContent.PrefixType<PrefixReinforced>()))
            {
                return (int)(item.defense * PrefixBalance.REINFORCED_DEFENSE_AMP);
            }
                
            return 0;
        });
    }

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        int defenseAdded = (int)(item.defense * PrefixBalance.REINFORCED_DEFENSE_AMP);
        
        yield return new TooltipLine(Mod, "newLine", Description.Format(MathF.Round(PrefixBalance.REINFORCED_DEFENSE_AMP * 100, 2)))
        {
            IsModifier = true
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