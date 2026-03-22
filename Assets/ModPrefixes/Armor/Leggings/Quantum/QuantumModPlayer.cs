using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Chestplate.Dashing;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Leggings.Quantum;

public class QuantumModPlayer : ModPlayer
{
    public bool HasQuantumLeggings;
    public int QuantumDashCooldown = 0;

    public override void ResetEffects()
    {
        HasQuantumLeggings = false;
    }

    public override void PreUpdateMovement()
    {
        if (QuantumDashCooldown > 0) QuantumDashCooldown--;

        if (!HasQuantumLeggings) return;

        // Detect the exact frame a vanilla dash starts
        if (Player.dashDelay == -1 && QuantumDashCooldown == 0 && !Player.mount.Active)
        {
            PerformQuantumTeleport();
        }
    }

    private void PerformQuantumTeleport()
    {
        Player.dashTime = 0; 
        Player.GetModPlayer<DashingModPlayer>().DashDuration = 0;
        
        Vector2 startPos = Player.position;
        Vector2 direction = Player.Center.DirectionTo(Main.MouseWorld);
        Vector2 rawTargetPos = startPos + (direction * PrefixBalance.QUANTUM_TELEPORT_DISTANCE);
        Vector2 finalTargetPos = FindForgivingPosition(rawTargetPos, Player.width, Player.height);
        
        int afterimageDamage = Player.statDefense * PrefixBalance.QUANTUM_AFTERIMAGE_DEFENSE_MULT; 

        Projectile.NewProjectile(
            Player.GetSource_FromThis(), 
            Player.Center, 
            Vector2.Zero, 
            ModContent.ProjectileType<QuantumAfterimageProjectile>(), 
            afterimageDamage, 
            5f, 
            Player.whoAmI
        );
        
        Player.Teleport(finalTargetPos, 1);
        
        float trueMomentum = Player.oldVelocity.Length();
        float exitSpeed = System.Math.Max(trueMomentum, 15f);
        
        Player.velocity = direction * exitSpeed;
        Player.fallStart = (int)(Player.position.Y / 16f);
        
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, Player.whoAmI, finalTargetPos.X, finalTargetPos.Y, 1);
        }
        
        // TODO: Play custom teleport sound effect here!
        // Terraria.Audio.SoundEngine.PlaySound(SoundID.Item8 with { Pitch = 0.5f }, Player.Center);
        
        QuantumDashCooldown = PrefixBalance.QUANTUM_DASH_COOLDOWN_TICKS;
        Player.dashDelay = PrefixBalance.QUANTUM_DASH_COOLDOWN_TICKS; 
    }

    private Vector2 FindForgivingPosition(Vector2 target, int width, int height)
    {
        if (!Collision.SolidCollision(target, width, height))
            return target;
            
        int searchRadius = PrefixBalance.QUANTUM_FORGIVING_SEARCH_RADIUS_BLOCKS * 16;
        int step = 8; 
        
        for (int r = step; r <= searchRadius; r += step)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;
                    
                    Vector2 testPos = target + new Vector2(i * r, j * r);
                    if (!Collision.SolidCollision(testPos, width, height))
                    {
                        return testPos;
                    }
                }
            }
        }
        
        return target; 
    }
}