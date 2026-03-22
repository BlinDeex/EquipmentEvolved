using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.Inevitable;

public class PrefixInevitable : BaseEvolvedPrefix, ISpecializedPrefix, IWorkInProgressPrefix, ILegacyPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.AnyArmor;
    
    public LocalizedText SetBonus { get; private set; }
    
    protected override void OnSetStaticDefaults()
    {
        SetBonus = GetLoc(nameof(SetBonus));
    }

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        int damagePercent = (int)(PrefixBalance.INEVITABLE_PIECE_DAMAGE_MULT * 100);
        int seconds = PrefixBalance.INEVITABLE_CLOCK_TICKS / 60;
        int bossDamagePercent = (int)(PrefixBalance.INEVITABLE_BOSS_MAX_HP_DAMAGE_PERCENT * 100);
        int cooldownSeconds = PrefixBalance.INEVITABLE_COOLDOWN_TICKS / 60;

        yield return new TooltipLine(Mod, "InevitableDesc", Description.Format(damagePercent))
        {
            IsModifier = true
        };

        yield return new TooltipLine(Mod, "InevitableSetBonus", SetBonus.Format(seconds, bossDamagePercent, cooldownSeconds))
        {
            IsModifier = true,
        };
    }
}