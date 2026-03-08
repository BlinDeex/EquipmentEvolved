using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.Phalanx;

public class PrefixPhalanx : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public static LocalizedText SetBonus { get; private set; }

    public static LocalizedText DescDamage { get; private set; }

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Phalanx", "DisplayName");

    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Headwear | SpecializedPrefixType.Chestplate | SpecializedPrefixType.Leggings;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    }


    public override void SetStaticDefaults()
    {
        SetBonus = LocalizationManager.GetPrefixLocalization(this, "Phalanx", nameof(SetBonus));
        DescDamage = LocalizationManager.GetSharedLocalizedText(LocalizationManager.XDamageAdded);
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        TooltipLine newLine = new(Mod, "newLine", DescDamage.Format(MathF.Round((PrefixBalance.PHALANX_DAMAGE_INCREASE - 1) * 100, 1)))
        {
            IsModifier = true
        };

        bool setBonusActive = Main.LocalPlayer.GetModPlayer<PhalanxArmorPlayer>().PhalanxSetBonus;


        TooltipLine newLine2 = new(Mod, "newLine2", SetBonus.Format(MathF.Round((int)(PrefixBalance.PHALANX_REACT_COOLDOWN_TICKS / 60f))))
        {
            IsModifier = true,
            IsModifierBad = !setBonusActive
        };

        yield return newLine;
        yield return newLine2;
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        player.GetModPlayer<PhalanxArmorPlayer>().PhalanxPiecesEquipped++;
    }
}