using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Magic.ManaCharged;

public class ManaChargedGlobalItem : GlobalItem
{
    public override bool InstancePerEntity => true;
    public float DamageAdded { get; private set; }

    public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
    {
        if (!item.HasPrefix(ModContent.PrefixType<PrefixManaCharged>())) return;

        float multiplier = (float)player.statMana / player.statManaMax2;
        float damageAdded = multiplier * PrefixBalance.TIDAL_MAX_DAMAGE_GAIN;
        damage.Base += damageAdded * item.damage;
        DamageAdded = damageAdded;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (!item.HasPrefix(ModContent.PrefixType<PrefixManaCharged>())) return;

        TooltipLine newLine = new(Mod, "newLine", LocalizationManager.GetSharedLocalizedText(LocalizationManager.XDamageAdded).Format(Math.Round(DamageAdded * 100, 2)))
        {
            IsModifier = true
        };

        tooltips.Add(newLine);
    }
}