using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.Kinetic;

public class KineticGlobalItem : GlobalItem
{
    public override bool InstancePerEntity => true;

    public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
    {
        if (item.HasPrefix(ModContent.PrefixType<PrefixKinetic>()))
        {
            float currentSpeed = player.velocity.Length();
            float momentumBonus = currentSpeed * PrefixBalance.KINETIC_VELOCITY_DAMAGE_SCALAR;

            if (momentumBonus > 2.5f) momentumBonus = 2.5f;

            damage *= 1f + momentumBonus;
        }
    }
}