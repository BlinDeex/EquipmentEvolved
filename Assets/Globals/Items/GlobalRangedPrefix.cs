using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.InstancedGlobalItems;
using EquipmentEvolved.Assets.ModPrefixes.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Globals.Items;

public class GlobalRangedPrefix : GlobalItem
{
    public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity,
        ref int type,
        ref int damage, ref float knockback)
    {
        if (item.prefix == ModContent.PrefixType<PrefixControlled>())
            velocity *= PrefixBalance.CONTROLLED_BULLET_VELOCITY;
    }

    public override bool? CanChooseAmmo(Item weapon, Item ammo, Player player)
    {
        if (weapon.prefix == ModContent.PrefixType<PrefixAdaptable>()) return true;

        if (weapon.prefix == ModContent.PrefixType<PrefixControlled>())
        {
            InstancedRangedPrefix weapInfo = weapon.GetGlobalItem<InstancedRangedPrefix>();
            if (weapInfo.CurrentBurstCooldown <= PrefixBalance.CONTROLLED_BURST_COOLDOWN_TICKS) return false;
        }

        return null;
    }
}