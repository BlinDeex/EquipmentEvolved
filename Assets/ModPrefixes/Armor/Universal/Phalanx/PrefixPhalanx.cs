using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using EquipmentEvolved.Assets.Stats.Combat;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.Phalanx;

public class PrefixPhalanx : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Headwear | SpecializedPrefixType.Chestplate | SpecializedPrefixType.Leggings;

    public LocalizedText SetBonus { get; private set; }
    public LocalizedText DescDamage { get; private set; }

    protected override void OnSetStaticDefaults()
    {
        
        SetBonus = GetLoc(nameof(SetBonus));
        DescDamage = LocalizationManager.GetSharedLocalizedText(LocalizationManager.XDamageAdded);
    }

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        TooltipLine newLine = new(Mod, "newLine", DescDamage.Format(MathF.Round((PrefixBalance.PHALANX_DAMAGE_INCREASE - 1) * 100, 1)))
        {
            IsModifier = true
        };

        bool setBonusActive = Main.LocalPlayer.GetModPlayer<PhalanxArmorPlayer>().PhalanxSetBonus;

        TooltipLine newLine2 = new(Mod, "newLine2", SetBonus.Format(MathF.Round((int)(PrefixBalance.PHALANX_REACT_COOLDOWN_TICKS / 60f))))
        {
            IsModifier = true,
            IsModifierBad = !setBonusActive
        };

        yield return newLine;
        yield return newLine2;
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        player.GetModPlayer<PhalanxArmorPlayer>().PhalanxPiecesEquipped++;
        float bonus = PrefixBalance.PHALANX_DAMAGE_INCREASE - 1;
        player.GetModPlayer<StatPlayer>().AddStat(ModContent.GetInstance<DamageStat>(), bonus, StatSource.Armor);
    }
}