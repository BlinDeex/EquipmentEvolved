using System;
using System.IO;
using System.Linq;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Buffs;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Melee;
using EquipmentEvolved.Assets.Projectiles;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
// Needed for MessageType enum

namespace EquipmentEvolved.Assets.ModPlayers;

public class PrefixPlayer : ModPlayer
{
    public float ManaSurgeMultiplier { get; set; }

    public float UntouchableDamageIncrease { get; set; }
    
    /// <summary>
    /// only stored when maximum increase is reached
    /// </summary>
    private Item untouchableBuffedWeapon;
    private int untouchableBuffedWeaponOldUseTime;
    private int untouchableBuffedWeaponOldAnimTime;

    public bool HasReforgedItemInv { get; private set; }

    private int tickTimer;

    private readonly SoundStyle manaSurgeDeathSS = new($"{nameof(EquipmentEvolved)}/Assets/Sounds/InvertedExplosion");

    // --- SYNCING START ---

    public override void CopyClientState(ModPlayer targetCopy)
    {
        PrefixPlayer clone = (PrefixPlayer)targetCopy;
        clone.ManaSurgeMultiplier = ManaSurgeMultiplier;
    }

    public override void SendClientChanges(ModPlayer clientPlayer)
    {
        PrefixPlayer clone = (PrefixPlayer)clientPlayer;
        
        // Sync if the value changes significantly (to save bandwidth) or hits 0
        if (Math.Abs(ManaSurgeMultiplier - clone.ManaSurgeMultiplier) > 0.02f)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)MessageType.SyncPrefixPlayer); // You must add this to your MessageType enum
            packet.Write((byte)Player.whoAmI);
            packet.Write(ManaSurgeMultiplier);
            packet.Send();
        }
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        ModPacket packet = Mod.GetPacket();
        packet.Write((byte)MessageType.SyncPrefixPlayer);
        packet.Write((byte)Player.whoAmI);
        packet.Write(ManaSurgeMultiplier);
        packet.Send(toWho, fromWho);
    }

    public void ReceiveSync(BinaryReader reader)
    {
        ManaSurgeMultiplier = reader.ReadSingle();
    }

    // --- SYNCING END ---
    
    public override void ModifyHurt(ref Player.HurtModifiers modifiers)
    {
        // Apply damage amp regardless of whether it is fully charged or building up
        if (UntouchableDamageIncrease > 0)
        {
            // Example: If Increase is 0.5 (50%), damage becomes 1.5x
            modifiers.FinalDamage *= (1f + UntouchableDamageIncrease);
        }
    }
    
    
    public override void OnHurt(Player.HurtInfo info)
    {
        // Reset if we have ANY progress, not just when fully charged
        if (UntouchableDamageIncrease > 0 || untouchableBuffedWeapon != null)
        {
            ResetUntouchable();
        }
    }

    private void ResetUntouchable()
    {
        // Revert weapon stats if they were currently buffed
        if (untouchableBuffedWeapon != null)
        {
            untouchableBuffedWeapon.useTime = untouchableBuffedWeaponOldUseTime;
            untouchableBuffedWeapon.useAnimation = untouchableBuffedWeaponOldAnimTime;
            untouchableBuffedWeapon = null;
        }

        // Wipe the progress
        UntouchableDamageIncrease = 0;
    }

    public override void PostUpdateEquips()
    {
        ChaoticRollPool.Tick(Player);

        UntouchableTick();

        tickTimer++;

        if (tickTimer % 60 == 0)
        {
            HasReforgedItemInv = HasReforgedItemInInventory();
        }
    }

    private bool HasReforgedItemInInventory()
    {
        return Player.inventory.Any(item => item.prefix != 0);
    }

    private void UntouchableTick()
    {
        if (Player.HeldItem.prefix == ModContent.PrefixType<PrefixUntouchable>())
            UntouchableDamageIncrease += PrefixBalance.RAZORS_EDGE_INCREASE_PER_TICK;
        else
            UntouchableDamageIncrease = 0;
        
        UntouchableDamageIncrease =
            MathHelper.Clamp(UntouchableDamageIncrease, 0f, PrefixBalance.RAZORS_EDGE_MAX_INCREASE);

        bool maxIncrease = Math.Abs(UntouchableDamageIncrease - PrefixBalance.RAZORS_EDGE_MAX_INCREASE) < 0.01f;
        
        if (untouchableBuffedWeapon != null) UntouchableEffects();
        
        if (maxIncrease && untouchableBuffedWeapon == null)
        {
            Item heldItem = Player.HeldItem;
            untouchableBuffedWeaponOldUseTime = heldItem.useTime;
            untouchableBuffedWeaponOldAnimTime = heldItem.useAnimation;
            untouchableBuffedWeapon = heldItem;
            int newUseTime = (int)(Player.HeldItem.useTime * PrefixBalance.RAZORS_EDGE_SWING_DECREASE);
            if (newUseTime == 0) newUseTime = 1;
            heldItem.useTime = newUseTime;
            heldItem.useAnimation = newUseTime;
            return;
        }
        
        if (!maxIncrease && untouchableBuffedWeapon != null)
        {
            untouchableBuffedWeapon.useTime = untouchableBuffedWeaponOldUseTime;
            untouchableBuffedWeapon.useAnimation = untouchableBuffedWeaponOldAnimTime;
            untouchableBuffedWeapon = null;
        }
    }

    private void UntouchableEffects()
    {
        Player.yoraiz0rEye = 7;
    }

    public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust,
        ref PlayerDeathReason damageSource)
    {
        if (Player.HasBuff<SacrificialBuff>())
        {
            Player.immuneTime = Player.numMinions * PrefixBalance.SACRIFICIAL_IMMUNE_FRAMES_PER_MINION;
            Player.numMinions = 0;
            Player.ClearBuff(ModContent.BuffType<SacrificialBuff>());
            Player.GetModPlayer<WhipPlayer>().SacrificialCooldown = PrefixBalance.SACRIFICIAL_COOLDOWN_TICKS;
            return false;
        }

        return damageSource != MiscStuff.GetManaSurgeDeath(Main.LocalPlayer.name) || ManaSurgeDeath();
    }

    private bool ManaSurgeDeath()
    {
        int damageEffect = Main.rand.Next(2000, 6000);
        HurtEffect(damageEffect, Main.LocalPlayer);
        SoundEngine.PlaySound(manaSurgeDeathSS, Main.LocalPlayer.position); // Fixed: Play at player position

        Vector2 playerCenter = Main.LocalPlayer.Center;

        int nodes = 10;

        for (int i = 0; i < nodes; i++)
        {
            Vector2 nodePos = UtilMethods.RandomPointInCircle(playerCenter.X, playerCenter.Y, 4f, Main.rand);
            int projectiles = Main.rand.Next(10, 20);
            float variation = Main.rand.NextFloat(-0.2f, 0.2f);
            float velMult = 10f + variation;

            for (int j = 0; j < projectiles; j++)
            {
                Vector2 projPos = UtilMethods.RandomPointInCircle(nodePos.X, nodePos.Y, 1f, Main.rand);
                Vector2 dir = (Main.LocalPlayer.Hitbox.Center() - projPos).SafeNormalize(Vector2.Zero);
                Vector2 velocity = dir * velMult;

                Projectile.NewProjectile(new EntitySource_Death(Main.LocalPlayer, "InvertedPrefix_Explosion"), projPos,
                    velocity,
                    ModContent.ProjectileType<InvertedProjectile>(), 20, 0, Main.LocalPlayer.whoAmI);
            }
        }

        return true;
    }

    public void HurtEffect(int damageAmount, Player player, bool broadcast = true)
    {
        CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height),
            CombatText.DamagedHostileCrit, damageAmount);
        if (broadcast && Main.netMode == NetmodeID.MultiplayerClient && Main.LocalPlayer.whoAmI == Main.myPlayer)
            NetMessage.SendData(MessageID.HurtPlayer, -1, -1, null, Main.LocalPlayer.whoAmI, damageAmount);
    }
}