using System;
using EquipmentEvolved.Assets.Balance;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.Gravitic;

public class GraviticGlobalItem : GlobalItem
{
    public override bool InstancePerEntity => true;

    /// <summary>
    /// This hook fires every single frame (tick) that the player is swinging the weapon.
    /// </summary>
    public override void UseItemFrame(Item item, Player player)
    {
        if (item.prefix != ModContent.PrefixType<PrefixGravitic>()) return;

        if (player.whoAmI != Main.myPlayer) return;
        
        if (player.itemAnimation == player.itemAnimationMax)
        {
            // TODO: Play custom gravitational wave sound here
        }
        
        float calculatedRadius = PrefixBalance.GRAVITIC_BASE_PULL_RADIUS + (item.useTime * PrefixBalance.GRAVITIC_RADIUS_PER_USE_TIME);
        float combinedScale = item.scale * player.GetAdjustedItemScale(item);
        float currentRadius = Math.Clamp(calculatedRadius * combinedScale, 0f, PrefixBalance.GRAVITIC_MAX_PULL_RADIUS);
        
        float currentPullSpeed = PrefixBalance.GRAVITIC_BASE_PULL_SPEED * (float)Math.Pow(PrefixBalance.GRAVITIC_SPEED_EXPONENT, item.useTime);
        currentPullSpeed = Math.Clamp(currentPullSpeed, 0f, PrefixBalance.GRAVITIC_MAX_PULL_SPEED);

        float swingProgress = (float)player.itemAnimation / player.itemAnimationMax;

        if (player.itemAnimation <= 1)
        {
            if (Terraria.Graphics.Effects.Filters.Scene["EquipmentEvolved:GraviticWave"].IsActive())
            {
                Terraria.Graphics.Effects.Filters.Scene["EquipmentEvolved:GraviticWave"].Opacity = 0f;
                Terraria.Graphics.Effects.Filters.Scene["EquipmentEvolved:GraviticWave"].Deactivate();
            }
        }
        else
        {
            if (!Terraria.Graphics.Effects.Filters.Scene["EquipmentEvolved:GraviticWave"].IsActive())
            {
                Terraria.Graphics.Effects.Filters.Scene.Activate("EquipmentEvolved:GraviticWave");
            }
            
            Terraria.Graphics.Effects.Filters.Scene["EquipmentEvolved:GraviticWave"].Opacity = 1f;

            Terraria.Graphics.Effects.Filters.Scene["EquipmentEvolved:GraviticWave"].GetShader().UseTargetPosition(player.MountedCenter).UseProgress(swingProgress)
                .UseOpacity(currentRadius).UseIntensity(currentPullSpeed * PrefixBalance.GRAVITIC_VISUAL_DISTORTION_MULT);
        }

        foreach (NPC npc in Main.ActiveNPCs)
        {
            if (!npc.active || npc.friendly || npc.dontTakeDamage || npc.lifeMax <= 5) continue;
            
            if (npc.knockBackResist <= 0f) continue;

            float distance = Vector2.Distance(player.Center, npc.Center);
            
            if (distance <= currentRadius && distance > 40f) 
            {
                Vector2 pullDirection = (player.Center - npc.Center).SafeNormalize(Vector2.Zero);
                
                float actualPull = currentPullSpeed * npc.knockBackResist;
                
                npc.position += pullDirection * actualPull;
                
                if (Main.netMode != Terraria.ID.NetmodeID.SinglePlayer)
                {
                    NetMessage.SendData(Terraria.ID.MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                }
            }
        }
    }
}