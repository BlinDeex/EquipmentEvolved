using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.CharmsModule.Data;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.Sealed;

public class PrefixSealed : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;
    public override float ReforgeMultiplier => PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.MeleeWeapon | SpecializedPrefixType.Whip;

    public LocalizedText HiddenDesc { get; private set; }
    public LocalizedText Instruction { get; private set; }
    public LocalizedText LockedWarning { get; private set; }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults(); 
        HiddenDesc = GetLoc(nameof(HiddenDesc));
        Instruction = GetLoc(nameof(Instruction));
        LockedWarning = GetLoc(nameof(LockedWarning));
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
                string statText = roll.GetTooltip() + " [Sealed]";

                string statName = roll.Stat?.Name ?? "Unknown";

                yield return new TooltipLine(Mod, $"SealedStat_{statName}", statText)
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