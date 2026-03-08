using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Accessory;

public class PrefixWarlord : ModPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Warlord", "DisplayName");

    public static LocalizedText Desc { get; private set; }

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ACCESSORY_REFORGING_MULTIPLIER;
    }

    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this, "Warlord", nameof(Desc));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        TooltipLine newLine = new(Mod, "newLine", Desc.Format(PrefixBalance.WARLORD_ADDITIONAL_MINIONS))
        {
            IsModifier = true
        };


        yield return newLine;
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        StatPlayer statPlayer = player.GetModPlayer<StatPlayer>();
        statPlayer.AdditionalMinions += statPlayer.CalculateStatBonus(PrefixBalance.WARLORD_ADDITIONAL_MINIONS, StatSource.AccessoryReforge);
    }
}