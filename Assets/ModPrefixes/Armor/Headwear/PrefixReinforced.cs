using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Buffs;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPlayers;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Headwear;

public class PrefixReinforced : ModPrefix, ISpecializedPrefix
{
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Headwear;
    
    public override PrefixCategory Category => PrefixCategory.Accessory;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    }

    public static LocalizedText Desc { get; private set; }
    public static LocalizedText NoSetBonus { get; private set; }
    
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"Reinforced", "DisplayName");

    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this,"Reinforced", nameof(Desc));
        NoSetBonus = SharedLocalization.GetSharedLocalizedText(SharedLocalization.NoArmorSetBonus);
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        int defenseAdded = (int)(item.defense * PrefixBalance.REINFORCED_DEFENSE_AMP);
        TooltipLine newLine = new TooltipLine(Mod, "newLine", Desc.Format(defenseAdded, MathF.Round(PrefixBalance.REINFORCED_DEFENSE_AMP * 100, 2)))
        {
            IsModifier = true
        };
        
        TooltipLine newLine2 = new TooltipLine(Mod, "newLine2", NoSetBonus.Value)
        {
            IsModifier = true,
            IsModifierBad = true
        };

        yield return newLine;
        yield return newLine2;
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        if (player.HasBuff<ArmorAbilityCooldownBuff>()) return;

        Item item = player.armor[0];
        int defenseAdded = (int)(item.defense * PrefixBalance.REINFORCED_DEFENSE_AMP);

        player.GetModPlayer<StatPlayer>().FlatDefense += defenseAdded;
    }
}