using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Delayed;

public class DelayedPrefixPlayer : ModPlayer
{
    public int PulseTimer;

    public override void PostUpdate()
    {
        Item heldItem = Player.inventory[Player.selectedItem];
        bool holdingDelayedWeapon = heldItem != null && !heldItem.IsAir && heldItem.HasPrefix(ModContent.PrefixType<PrefixDelayed>());
        
        bool isShooting = Player.controlUseItem || Player.itemAnimation > 0;

        if (holdingDelayedWeapon && isShooting)
        {
            PulseTimer++;
            if (PulseTimer >= PrefixBalance.DELAYED_PULSE_TIMER)
            {
                TriggerDelayedSnap();
                PulseTimer = 0;
            }
        }
        else if (PulseTimer > 0)
        {
            TriggerDelayedSnap();
            PulseTimer = 0;
        }
    }

    private void TriggerDelayedSnap()
    {
        for (int i = 0; i < Main.maxProjectiles; i++)
        {
            Projectile proj = Main.projectile[i];
            if (proj.active && proj.owner == Player.whoAmI && proj.TryGetGlobalProjectile(out DelayedGlobalProjectile globalProj))
            {
                if (globalProj.IsDelayedReady)
                {
                    Vector2 startPos = proj.Center;
                    Vector2 endPos = globalProj.LaserEndPoint;
                    proj.Kill();
                    Projectile.NewProjectile(Player.GetSource_FromThis(), startPos, Vector2.Zero, ModContent.ProjectileType<DelayedLaserProjectile>(),
                        (int)(proj.damage * PrefixBalance.DELAYED_LASER_DAMAGE_MULT), proj.knockBack, Player.whoAmI, endPos.X, endPos.Y, proj.width);
                }
            }
        }
    }

    private static bool TryGetIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 intersection)
    {
        intersection = Vector2.Zero;
        float denominator = (p4.Y - p3.Y) * (p2.X - p1.X) - (p4.X - p3.X) * (p2.Y - p1.Y);

        if (denominator == 0) return false;

        float uA = ((p4.X - p3.X) * (p1.Y - p3.Y) - (p4.Y - p3.Y) * (p1.X - p3.X)) / denominator;
        float uB = ((p2.X - p1.X) * (p1.Y - p3.Y) - (p2.Y - p1.Y) * (p1.X - p3.X)) / denominator;

        if (uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1)
        {
            intersection = new Vector2(p1.X + uA * (p2.X - p1.X), p1.Y + uA * (p2.Y - p1.Y));
            return true;
        }

        return false;
    }
}