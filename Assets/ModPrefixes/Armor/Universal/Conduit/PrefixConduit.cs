using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.Conduit;

public class PrefixConduit : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.AnyArmor;

    public LocalizedText SetBonus { get; private set; }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        SetBonus = GetLoc(nameof(SetBonus));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        TooltipLine newLine = new(Mod, "newLine", Description.Format(PrefixBalance.CONDUIT_MANA_PER_PIECE))
        {
            IsModifier = true
        };

        bool setBonusActive = Main.LocalPlayer.GetModPlayer<ConduitModPlayer>().ConduitSetBonus;

        TooltipLine newLine2 = new(Mod, "newLine2", SetBonus.ToString())
        {
            IsModifier = true,
            IsModifierBad = !setBonusActive
        };

        yield return newLine;
        yield return newLine2;
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        player.GetModPlayer<ConduitModPlayer>().ConduitPieces++;
        player.statManaMax2 += PrefixBalance.CONDUIT_MANA_PER_PIECE;
    }
}