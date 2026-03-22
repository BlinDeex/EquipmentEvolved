using System;
using EquipmentEvolved.Assets.Balance;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Headwear.Phasing;

public class PhasingModPlayer : ModPlayer
{
    public bool HasPhasing;
    
    private Vector2 _oldPosition;
    private Vector2 _oldVelocity;
    private int _oldDashDelay;

    public override void ResetEffects()
    {
        HasPhasing = false;
    }

    public override void PostUpdateMiscEffects()
    {
        if (!HasPhasing || Player.dead || !Player.active)
        {
            _oldPosition = Player.position;
            _oldVelocity = Player.velocity;
            _oldDashDelay = Player.dashDelay;
            return;
        }

        // 1. Detect Dashes
        // dashDelay becomes -1 the exact frame a vanilla (or standard modded) dash starts
        if (Player.dashDelay == -1 && _oldDashDelay != -1)
        {
            GrantIframes(PrefixBalance.PHASING_DASH_IFRAMES);
        }

        // 2. Detect Teleports (Dynamic Velocity Tracking)
        Vector2 actualDisplacement = Player.position - _oldPosition;
        
        // We check the highest velocity between the current and previous frame. 
        // This prevents false positives if the player hits a wall and their velocity drops to 0 mid-frame!
        float maxExpectedMovement = Math.Max(Player.velocity.Length(), _oldVelocity.Length());

        // If the player moved significantly further than their velocity allows, they teleported.
        // The 32f buffer (2 blocks) allows for Terraria's instant slope/stair-stepping mechanics without triggering.
        if (actualDisplacement.Length() > maxExpectedMovement + 32f)
        {
            GrantIframes(PrefixBalance.PHASING_TELEPORT_IFRAMES);
        }

        // Store the state for the next frame's comparison
        _oldPosition = Player.position;
        _oldVelocity = Player.velocity;
        _oldDashDelay = Player.dashDelay;
    }

    private void GrantIframes(int durationTicks)
    {
        // Only grant the i-frames if it is an upgrade. 
        if (Player.immuneTime < durationTicks)
        {
            Player.immune = true;
            Player.immuneTime = durationTicks;
        }
    }
}