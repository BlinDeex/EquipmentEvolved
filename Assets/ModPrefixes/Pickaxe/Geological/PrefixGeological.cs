using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Pickaxe.Geological;

public class PrefixGeological : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Melee;

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Geological", "DisplayName");

    public static LocalizedText Description { get; private set; }
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Pickaxe;

    public override void SetStaticDefaults()
    {
        Description = LocalizationManager.GetPrefixLocalization(this, "Geological", nameof(Description));
    }

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.TOOL_REFORGING_MULTIPLIER;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        int chance = (int)(PrefixBalance.GEOLOGICAL_GEM_DROP_CHANCE * 100);

        yield return new TooltipLine(Mod, "GeologicalDescription", Description.Format(chance))
        {
            OverrideColor = Color.LightGoldenrodYellow
        };
    }
}