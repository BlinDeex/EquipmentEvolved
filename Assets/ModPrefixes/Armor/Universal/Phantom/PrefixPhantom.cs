using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.Chrono;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.Phantom;

public class PrefixPhantom : BaseEvolvedPrefix, ISpecializedPrefix, IExperimentalPrefix
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
        TooltipLine newLine = new(Mod, "newLine", Description.Format(MathF.Round(PrefixBalance.PHANTOM_TRUE_DAMAGE_AMP * 100, 2)))
        {
            IsModifier = true
        };
        
        bool setBonusActive = Main.LocalPlayer.GetModPlayer<PhantomArmorPlayer>().PhantomSetBonus;
        TooltipLine newLine2 = new(Mod, "newLine2", SetBonus.Format(Math.Round(PrefixBalance.CHRONO_ABILITY_LENGTH / 60f, 2)))
        {
            IsModifier = true,
            IsModifierBad = !setBonusActive
        };

        yield return newLine;
        yield return newLine2;
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        player.GetModPlayer<PhantomArmorPlayer>().PhantomPiecesEquipped++;
    }
}