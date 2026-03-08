using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Core;

public class CoreGlobalItem : GlobalItem
{
    public override void SetDefaults(Item entity)
    {
        entity.damage = (int)(entity.damage * PrefixBalance.GLOBAL_WEAPON_DAMAGE_NERF_MUL);
        if (entity.IsArmor()) entity.defense = (int)(entity.defense * PrefixBalance.GLOBAL_ARMOR_DEFENSE_NERF_MUL);

        entity.accessory = entity.accessory || entity.IsArmor();
    }

    public override bool CanEquipAccessory(Item item, Player player, int slot, bool modded)
    {
        return !item.IsArmor();
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (!item.accessory || item.prefix == 0 || tooltips.Count == 0) return;

        TooltipLine title = tooltips[0];
        title.OverrideColor = Main.DiscoColor;
    }

    public override bool AllowPrefix(Item item, int pre)
    {
        if (!PrefixValidator.CanApplyPrefix(item, pre)) return false;

        return base.AllowPrefix(item, pre);
    }
}