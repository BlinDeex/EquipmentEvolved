using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPlayers;
using EquipmentEvolved.Assets.ModPlayers.Armor.Universal;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Universal;

public class PrefixChrono : ModPrefix, ISpecializedPrefix
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
    
    public static LocalizedText Desc { get; private set; }
    
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"Chrono", "DisplayName");


    public override void SetStaticDefaults()
    {
        SetBonus = LocalizationManager.GetPrefixLocalization(this,"Chrono", nameof(SetBonus));
        Desc = LocalizationManager.GetPrefixLocalization(this,"Chrono", nameof(Desc));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        TooltipLine newLine = new TooltipLine(Mod, "newLine",
            Desc.Format(MathF.Round((PrefixBalance.CHRONO_MOVEMENT_SPEED - 1) * 100, 1)))
        {
            IsModifier = true
        };
        bool setBonusActive = Main.LocalPlayer.GetModPlayer<ChronoArmorPlayer>().ChronoSetBonus;
        
        
        TooltipLine newLine2 = new TooltipLine(Mod, "newLine2",
            SetBonus.Format(Math.Round(PrefixBalance.CHRONO_ABILITY_LENGTH / 60f, 1)))
        {
            IsModifier = true,
            IsModifierBad = !setBonusActive
        };
        
        yield return newLine;
        yield return newLine2;
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        player.GetModPlayer<ChronoArmorPlayer>().ChronoPiecesEquipped++;
        player.GetModPlayer<StatPlayer>().MovementSpeedMul += PrefixBalance.CHRONO_MOVEMENT_SPEED - 1;
    }
}