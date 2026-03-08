using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.Conduit;

public class PrefixConduit : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;

    public static LocalizedText SetBonus { get; private set; }
    public static LocalizedText Desc { get; private set; }

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Conduit", "DisplayName");

    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.AnyArmor;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    }

    public override void SetStaticDefaults()
    {
        SetBonus = LocalizationManager.GetPrefixLocalization(this, "Conduit", nameof(SetBonus));
        Desc = LocalizationManager.GetPrefixLocalization(this, "Conduit", nameof(Desc));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        TooltipLine newLine = new(Mod, "newLine", Desc.Format(PrefixBalance.CONDUIT_MANA_PER_PIECE))
        {
            IsModifier = true
        };

        bool setBonusActive = Main.LocalPlayer.GetModPlayer<ConduitModPlayer>().ConduitSetBonus;

        TooltipLine newLine2 = new(Mod, "newLine2", SetBonus.ToString())
        {
            IsModifier = true,
            IsModifierBad = !setBonusActive
        };

        yield return newLine;
        yield return newLine2;
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        player.GetModPlayer<ConduitModPlayer>().ConduitPieces++;
        player.statManaMax2 += PrefixBalance.CONDUIT_MANA_PER_PIECE;
    }
}