using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.RaidBoss;

public class PrefixRaidBoss : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;

    public static LocalizedText SetBonus { get; private set; }
    public static LocalizedText Desc { get; private set; }

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "RaidBoss", "DisplayName");

    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.AnyArmor;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    }

    public override void SetStaticDefaults()
    {
        SetBonus = LocalizationManager.GetPrefixLocalization(this, "RaidBoss", nameof(SetBonus));
        Desc = LocalizationManager.GetPrefixLocalization(this, "RaidBoss", nameof(Desc));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        TooltipLine newLine = new(Mod, "newLine", Desc.Format(MathF.Round((PrefixBalance.RAID_BOSS_PIECE_DEFENSE_MULT - 1f) * 100, 1)))
        {
            IsModifier = true
        };

        bool setBonusActive = Main.LocalPlayer.GetModPlayer<RaidBossModPlayer>().RaidBossSetBonus;

        int mobilityPenalty = (int)(PrefixBalance.RAID_BOSS_MOBILITY_PENALTY * 100);
        int drBonus = (int)(PrefixBalance.RAID_BOSS_SET_DR_BONUS * 100);
        int setDefense = PrefixBalance.RAID_BOSS_SET_DEFENSE_BONUS;

        TooltipLine newLine2 = new(Mod, "newLine2", SetBonus.Format(mobilityPenalty, setDefense, drBonus))
        {
            IsModifier = true,
            IsModifierBad = !setBonusActive
        };

        yield return newLine;
        yield return newLine2;
    }
}