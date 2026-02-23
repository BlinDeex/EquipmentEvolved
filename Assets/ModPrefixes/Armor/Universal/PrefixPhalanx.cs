using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPlayers.Armor.Universal;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Universal;

public class PrefixPhalanx : ModPrefix, ISpecializedPrefix
{
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Headwear |
                                                          SpecializedPrefixType.Chestplate |
                                                          SpecializedPrefixType.Leggings;
    

    public override PrefixCategory Category => PrefixCategory.Accessory;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    }
    public static LocalizedText SetBonus { get; private set; }
    
    public static LocalizedText DescDamage { get; private set; }
    
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"Phalanx", "DisplayName");


    public override void SetStaticDefaults()
    {
        SetBonus = LocalizationManager.GetPrefixLocalization(this,"Phalanx", nameof(SetBonus));
        DescDamage = SharedLocalization.GetSharedLocalizedText(SharedLocalization.XDamageAdded);
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        TooltipLine newLine = new TooltipLine(Mod, "newLine",
            DescDamage.Format(MathF.Round((PrefixBalance.PHALANX_DAMAGE_INCREASE - 1) * 100, 1)))
        {
            IsModifier = true
        };

        bool setBonusActive = Main.LocalPlayer.GetModPlayer<PhalanxArmorPlayer>().PhalanxSetBonus;
        
        
        TooltipLine newLine2 = new TooltipLine(Mod, "newLine2",
            SetBonus.Format(MathF.Round((int)(PrefixBalance.PHALANX_REACT_COOLDOWN_TICKS / 60f))))
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