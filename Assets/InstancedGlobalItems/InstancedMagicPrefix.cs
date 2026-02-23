using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Globals.Items;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPlayers;
using EquipmentEvolved.Assets.ModPrefixes.Magic;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.InstancedGlobalItems;

public class InstancedMagicPrefix : GlobalItem
{
    public override bool InstancePerEntity => true;

    public float DamageAdded { get; private set; }

    public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
    {
        StatPlayer statPlayer = player.GetModPlayer<StatPlayer>();

        if (item.prefix == ModContent.PrefixType<PrefixManaCharged>())
        {
            float multiplier = (float)player.statMana / player.statManaMax2;
            float damageAdded = multiplier * PrefixBalance.MANA_CHARGED_MAX_DAMAGE_GAIN;
            damage.Base += damageAdded * item.damage;
            DamageAdded = damageAdded;
            return;
        }

        if (item.prefix == ModContent.PrefixType<PrefixChaotic>()) damage *= statPlayer.DamageMul;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (item.prefix == ModContent.PrefixType<PrefixManaCharged>())
        {
            TooltipLine newLine = new TooltipLine(Mod, "newLine",
                SharedLocalization.GetSharedLocalizedText(SharedLocalization.XDamageAdded)
                    .Format(Math.Round(DamageAdded * 100, 2)))
            {
                IsModifier = true
            };

            tooltips.Add(newLine);
            return;
        }

        if (item.prefix == ModContent.PrefixType<PrefixChaotic>())
        {
            tooltips.AddRange(GlobalMagicPrefix.CurrentChaoticTooltipLines);
            return;
        }
    }
}