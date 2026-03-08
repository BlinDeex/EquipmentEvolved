using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Accessory;

public class PrefixRevitalizing : ModPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Revitalizing", "DisplayName");

    public static LocalizedText Desc { get; private set; }

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ACCESSORY_REFORGING_MULTIPLIER;
    }

    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this, "Revitalizing", nameof(Desc));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        TooltipLine newLine = new(Mod, "newLine", Desc.Format(PrefixBalance.REVITALIZING_REGENERATION / 2))
        {
            IsModifier = true
        };

        yield return newLine;
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        if (player.TryGetModPlayer(out StatPlayer statPlayer)) statPlayer.Regen += statPlayer.CalculateStatBonus(PrefixBalance.REVITALIZING_REGENERATION, StatSource.AccessoryReforge);
    }
}