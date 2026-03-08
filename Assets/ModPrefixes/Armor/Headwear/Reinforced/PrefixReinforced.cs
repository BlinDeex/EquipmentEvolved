using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Core;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Headwear.Reinforced;

public class PrefixReinforced : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;

    public static LocalizedText Desc { get; private set; }
    public static LocalizedText NoSetBonus { get; private set; }

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Reinforced", "DisplayName");

    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Headwear;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    }

    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this, "Reinforced", nameof(Desc));
        NoSetBonus = LocalizationManager.GetSharedLocalizedText(LocalizationManager.NoArmorSetBonus);
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        int defenseAdded = (int)(item.defense * PrefixBalance.REINFORCED_DEFENSE_AMP);
        TooltipLine newLine = new(Mod, "newLine", Desc.Format(defenseAdded, MathF.Round(PrefixBalance.REINFORCED_DEFENSE_AMP * 100, 2)))
        {
            IsModifier = true
        };

        TooltipLine newLine2 = new(Mod, "newLine2", NoSetBonus.Value)
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