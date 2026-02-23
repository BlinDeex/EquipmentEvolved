using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Globals.GlobalNerf;

public class PrefixGlobalNerf : GlobalItem
{
    public override void SetDefaults(Item entity)
    {
        base.SetDefaults(entity);
        entity.damage = (int)(entity.damage * PrefixBalance.GLOBAL_WEAPON_DAMAGE_NERF_MUL);
        if(entity.IsArmor()) entity.defense = (int)(entity.defense * PrefixBalance.GLOBAL_ARMOR_DEFENSE_NERF_MUL);
    }
}