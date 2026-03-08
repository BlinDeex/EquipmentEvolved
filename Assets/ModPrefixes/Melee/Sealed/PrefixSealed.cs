using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.CharmsModule.Data;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.Sealed;

public class PrefixSealed : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;

    public static LocalizedText HiddenDesc { get; private set; }
    public static LocalizedText Instruction { get; private set; }
    public static LocalizedText LockedWarning { get; private set; }

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Sealed", "DisplayName");

    public SpecializedPrefixType SpecializedPrefixType =>
        SpecializedPrefixType.MeleeWeapon | SpecializedPrefixType.Whip;

    public override void SetStaticDefaults()
    {
        HiddenDesc = LocalizationManager.GetPrefixLocalization(this, "Sealed", nameof(HiddenDesc));
        Instruction = LocalizationManager.GetPrefixLocalization(this, "Sealed", nameof(Instruction));
        LockedWarning = LocalizationManager.GetPrefixLocalization(this, "Sealed", nameof(LockedWarning));
    }

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }

    public override void Apply(Item item)
    {
        if (item.TryGetGlobalItem(out SealedGlobalItem instanced))
        {
            instanced.IsSealed = true;
            instanced.IsRevealed = false;
            instanced.Rolls = SealedRollManager.GenerateRolls();
        }
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        if (!item.TryGetGlobalItem(out SealedGlobalItem instanced) || !instanced.IsSealed) yield break;

        if (!instanced.IsRevealed)
        {
            yield return new TooltipLine(Mod, "SealedHidden", HiddenDesc.Value) { OverrideColor = Color.Purple };
            yield return new TooltipLine(Mod, "SealedInstruction", Instruction.Value) { OverrideColor = Color.Gray };
        }
        else
        {
            foreach (CharmRoll roll in instanced.Rolls)
            {
                float displayStrength = roll.GetStrength();

                string statText = LocalizationManager.GetCharmText(roll.Stat).Format(displayStrength);

                yield return new TooltipLine(Mod, $"SealedStat_{roll.Stat}", statText)
                {
                    OverrideColor = Color.Gold,
                    IsModifier = true
                };
            }

            yield return new TooltipLine(Mod, "SealedWarning", LockedWarning.Value)
            {
                OverrideColor = Color.Red
            };
        }
    }
}