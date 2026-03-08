using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Controlled;

public class ControlledGlobalItem : GlobalItem
{
    public override bool InstancePerEntity => true;
    public int CurrentBurstCooldown { get; private set; }

    public override void HoldItem(Item item, Player player)
    {
        if (item.HasPrefix(ModContent.PrefixType<PrefixControlled>())) CurrentBurstCooldown--;
    }

    public override bool CanUseItem(Item item, Player player)
    {
        PrefixControlled(item, out bool canUse);
        return canUse;
    }

    public override bool? CanChooseAmmo(Item weapon, Item ammo, Player player)
    {
        if (!weapon.HasPrefix(ModContent.PrefixType<PrefixControlled>())) return null;

        if (CurrentBurstCooldown <= PrefixBalance.CONTROLLED_BURST_COOLDOWN_TICKS) return false;

        return null;
    }

    public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (item.HasPrefix(ModContent.PrefixType<PrefixControlled>())) velocity *= PrefixBalance.CONTROLLED_BULLET_VELOCITY;
    }

    private void PrefixControlled(Item item, out bool canUse)
    {
        if (item.HasPrefix(ModContent.PrefixType<PrefixControlled>()))
        {
            if (CurrentBurstCooldown <= 0) CurrentBurstCooldown = PrefixBalance.CONTROLLED_BURST_COOLDOWN_TICKS + PrefixBalance.CONTROLLED_BURST_DURATION_TICKS;

            if (CurrentBurstCooldown <= PrefixBalance.CONTROLLED_BURST_COOLDOWN_TICKS)
            {
                canUse = false;
                return;
            }
        }

        canUse = true;
    }
}