using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Core;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using EquipmentEvolved.Assets.Stats.Custom;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Chestplate.Void;

public class PrefixVoid : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Chestplate;

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        int damagePercent = (int)Math.Round(PrefixBalance.VOID_TRUE_DAMAGE_BONUS * 100);
        yield return new TooltipLine(Mod, "VoidDesc", Description.Format(damagePercent)) { IsModifier = true };
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        if (player.HasBuff<ArmorAbilityCooldownBuff>()) return;
        StatPlayer statPlayer = player.GetModPlayer<StatPlayer>();
        statPlayer.AddStat(ModContent.GetInstance<TrueDamageMulStat>(), PrefixBalance.VOID_TRUE_DAMAGE_BONUS, StatSource.Chestplate);
    }
}