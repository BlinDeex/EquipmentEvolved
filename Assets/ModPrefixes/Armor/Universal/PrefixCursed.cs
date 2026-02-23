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

public class PrefixCursed : ModPrefix, ISpecializedPrefix
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
    
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"Cursed", "DisplayName");


    public override void SetStaticDefaults()
    {
        SetBonus = LocalizationManager.GetPrefixLocalization(this,"Cursed", nameof(SetBonus));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        bool setBonusActive = Main.LocalPlayer.GetModPlayer<CursedArmorPlayer>().CursedSetBonus;
        
        TooltipLine newLine = new TooltipLine(Mod, "newLine",
            SharedLocalization.GetSharedLocalizedText(SharedLocalization.XIncreasedLifesteal).Format(Math.Round(PrefixBalance.CURSED_LIFESTEAL, 2)))
        {
            IsModifier = true,
        };
        
        TooltipLine newLine2 = new TooltipLine(Mod, "newLine2",
            SetBonus.Format(MathF.Round(PrefixBalance.CURSED_IGNORED_DAMAGE_PERCENT_THRESHOLD * 100, 2),
                Math.Round(PrefixBalance.CURSED_DAMAGE_TAKEN_PERCENT * 100, 2)))
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
        player.GetModPlayer<StatPlayer>().Lifesteal += PrefixBalance.CURSED_LIFESTEAL;
    }
}