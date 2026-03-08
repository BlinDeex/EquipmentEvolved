using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.Cursed;

public class PrefixCursed : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public static LocalizedText SetBonus { get; private set; }

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Cursed", "DisplayName");

    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.AnyArmor;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    }


    public override void SetStaticDefaults()
    {
        SetBonus = LocalizationManager.GetPrefixLocalization(this, "Cursed", nameof(SetBonus));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        bool setBonusActive = Main.LocalPlayer.GetModPlayer<CursedArmorPlayer>().CursedSetBonus;

        TooltipLine newLine = new(Mod, "newLine", LocalizationManager.GetSharedLocalizedText(LocalizationManager.XIncreasedLifesteal).Format(Math.Round(PrefixBalance.CURSED_LIFESTEAL, 2)))
        {
            IsModifier = true
        };

        TooltipLine newLine2 = new(Mod, "newLine2",
            SetBonus.Format(MathF.Round(PrefixBalance.CURSED_IGNORED_DAMAGE_PERCENT_THRESHOLD * 100, 2), Math.Round(PrefixBalance.CURSED_DAMAGE_TAKEN_PERCENT * 100, 2)))
        {
            IsModifier = true,
            IsModifierBad = !setBonusActive
        };

        yield return newLine;
        yield return newLine2;
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        player.GetModPlayer<CursedArmorPlayer>().CursedPiecesEquipped++;
        player.GetModPlayer<StatPlayer>().OnHitLifesteal += PrefixBalance.CURSED_LIFESTEAL;
    }
}