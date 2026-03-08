using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Pickaxe.VeinMiner;

public class PrefixVeinMiner : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "VeinMiner", "DisplayName");

    public static LocalizedText VeinMinerDesc { get; private set; }
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Pickaxe;


    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.TOOL_REFORGING_MULTIPLIER;
    }

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
    {
        useTimeMult *= PrefixBalance.VEIN_MINER_MINING_SPEED;
    }

    public override void SetStaticDefaults()
    {
        VeinMinerDesc = LocalizationManager.GetPrefixLocalization(this, "VeinMiner", nameof(VeinMinerDesc));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        TooltipLine newLine = new(Mod, "newLine", VeinMinerDesc.Value)
        {
            IsModifier = true
        };

        yield return newLine;
    }
}