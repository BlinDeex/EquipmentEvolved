using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.Symbiotic;

public class PrefixSymbiotic : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;

    public static LocalizedText Desc { get; private set; }
    public static LocalizedText CurseDesc { get; private set; }
    public static LocalizedText SetBonus { get; private set; }

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Symbiotic", "DisplayName");

    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.AnyArmor;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    }

    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this, "Symbiotic", nameof(Desc));
        CurseDesc = LocalizationManager.GetPrefixLocalization(this, "Symbiotic", nameof(CurseDesc));
        SetBonus = LocalizationManager.GetPrefixLocalization(this, "Symbiotic", nameof(SetBonus));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        TooltipLine descLine = new(Mod, "SymbioticDesc", Desc.Format(MathF.Round(CharmBalance.SYMBIOTIC_BASE_QUALITY_BOOST * 100, 1)))
        {
            IsModifier = true
        };

        TooltipLine curseLine = new(Mod, "SymbioticCurse", CurseDesc.Value)
        {
            IsModifier = true
        };

        bool setBonusActive = Main.LocalPlayer.GetModPlayer<SymbioticPlayer>().SymbioticPiecesEquipped >= 3;

        TooltipLine setBonusLine = new(Mod, "SymbioticSetBonus", SetBonus.Format(MathF.Round(CharmBalance.SYMBIOTIC_AUGMENTATION_CONSUME_BOOST * 100, 1)))
        {
            IsModifier = true,
            IsModifierBad = !setBonusActive
        };

        yield return descLine;
        yield return curseLine;
        yield return setBonusLine;
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        player.GetModPlayer<SymbioticPlayer>().SymbioticPiecesEquipped++;
    }
}