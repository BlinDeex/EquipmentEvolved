using System;
using System.IO;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Magic.Inverted;

public class InvertedModPlayer : ModPlayer
{
    private readonly SoundStyle manaSurgeDeathSS = new($"{nameof(EquipmentEvolved)}/Assets/Sounds/InvertedExplosion");
    public float ManaSurgeMultiplier { get; set; }
    
    public override void CopyClientState(ModPlayer targetCopy)
    {
        InvertedModPlayer clone = (InvertedModPlayer)targetCopy;
        clone.ManaSurgeMultiplier = ManaSurgeMultiplier;
    }

    public override void SendClientChanges(ModPlayer clientPlayer)
    {
        InvertedModPlayer clone = (InvertedModPlayer)clientPlayer;
        if (Math.Abs(ManaSurgeMultiplier - clone.ManaSurgeMultiplier) > 0.02f)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)MessageType.SyncInvertedModPlayer);
            packet.Write((byte)Player.whoAmI);
            packet.Write(ManaSurgeMultiplier);
            packet.Send();
        }
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        ModPacket packet = Mod.GetPacket();
        packet.Write((byte)MessageType.SyncInvertedModPlayer);
        packet.Write((byte)Player.whoAmI);
        packet.Write(ManaSurgeMultiplier);
        packet.Send(toWho, fromWho);
    }

    public void ReceiveSync(BinaryReader reader)
    {
        ManaSurgeMultiplier = reader.ReadSingle();
    }

    public override void PostUpdateBuffs()
    {
        if (Main.myPlayer != Player.whoAmI) return;

        if (!Player.HeldItem.HasPrefix(ModContent.PrefixType<PrefixInverted>())) return;

        int manaSicknessIndex = Player.FindBuffIndex(BuffID.ManaSickness);
        if (manaSicknessIndex == -1) return;

        int time = Player.buffTime[manaSicknessIndex];
        Player.DelBuff(manaSicknessIndex);
        Player.AddBuff(ModContent.BuffType<InvertedBuff>(), time);
    }

    public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
    {
        return damageSource != MiscStuff.GetManaSurgeDeath(Player.name) || ManaSurgeDeath();
    }

    private bool ManaSurgeDeath()
    {
        int damageEffect = Main.rand.Next(2000, 6000);
        HurtEffect(damageEffect, Player);
        SoundEngine.PlaySound(manaSurgeDeathSS, Player.position);

        Vector2 playerCenter = Player.Center;
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
                Vector2 dir = (Player.Hitbox.Center() - projPos).SafeNormalize(Vector2.Zero);
                Vector2 velocity = dir * velMult;

                Projectile.NewProjectile(new EntitySource_Death(Player, "InvertedPrefix_Explosion"), projPos, velocity, ModContent.ProjectileType<InvertedProjectile>(), 20, 0, Player.whoAmI);
            }
        }

        return true;
    }

    public void HurtEffect(int damageAmount, Player player, bool broadcast = true)
    {
        CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.DamagedHostileCrit, damageAmount);
        if (broadcast && Main.netMode == NetmodeID.MultiplayerClient && Player.whoAmI == Main.myPlayer) NetMessage.SendData(MessageID.HurtPlayer, -1, -1, null, Player.whoAmI, damageAmount);
    }
}