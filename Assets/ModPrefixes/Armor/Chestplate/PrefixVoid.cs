using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Chestplate;

public class PrefixVoid : ModPrefix, ISpecializedPrefix
{
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Chestplate;
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public static LocalizedText Desc { get; private set; }
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this, "Void", "DisplayName");

    public override void ModifyValue(ref float valueMult) => valueMult = PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    public override void SetStaticDefaults() => Desc = LocalizationManager.GetPrefixLocalization(this, "Void", nameof(Desc));

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        int damagePercent = (int)Math.Round(PrefixBalance.VOID_TRUE_DAMAGE_BONUS * 100);
        yield return new TooltipLine(Mod, "VoidDesc", Desc.Format(damagePercent)) { IsModifier = true };
    }
}