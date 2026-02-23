using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Pickaxe;

public class PrefixVeinMiner : ModPrefix, ISpecializedPrefix
{
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Pickaxe;

    public override PrefixCategory Category => PrefixCategory.AnyWeapon;
    
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"VeinMiner", "DisplayName");


    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.TOOL_REFORGING_MULTIPLIER;
    }

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult,
        ref float scaleMult,
        ref float shootSpeedMult, ref float manaMult, ref int critBonus)
    {
        useTimeMult *= PrefixBalance.VEIN_MINER_MINING_SPEED;
    }

    public static LocalizedText VeinMinerDesc { get; private set; }

    public override void SetStaticDefaults()
    {
        VeinMinerDesc = LocalizationManager.GetPrefixLocalization(this,"VeinMiner", nameof(VeinMinerDesc));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        TooltipLine newLine = new TooltipLine(Mod, "newLine",
            VeinMinerDesc.Value)
        {
            IsModifier = true
        };

        yield return newLine;
    }
}