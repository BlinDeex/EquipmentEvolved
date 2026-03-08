using System;
using System.IO;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Challenger;

public class ChallengerModPlayer : ModPlayer
{
    private int ticksUntilChallengerOrbSpawn;

    /// <summary>
    ///     This increases if player shoots orbs correctly, decreases/resets when bad orb was hit or unequips weapon
    /// </summary>
    public float ChallengerScore { get; private set; } = 1f;

    public bool ChallengerModeActive { get; private set; }

    public float ChallengerProjVelMultiplier { get; private set; } = 1f;
    public float ChallengerDamageMultiplier { get; private set; } = 1f;
    public float ChallengerDefenseMultiplier { get; private set; } = 1f;
    public float ChallengerCritMultiplier { get; private set; } = 1f;

    public void GoodOrbMiss()
    {
        ChallengerScore *= PrefixBalance.CHALLENGER_ORB_MISS_MULTIPLIER;
        ChallengerScore = Math.Clamp(ChallengerScore, 1f, 10f);
        MultiplyChallengerStats(PrefixBalance.CHALLENGER_ORB_MISS_MULTIPLIER);
    }


    public void OrbHit(ChallengerOrbType type, bool perfect, NPC victim, Projectile projectile)
    {
        bool goodOrb = type is ChallengerOrbType.Blue or ChallengerOrbType.Green or ChallengerOrbType.Yellow;

        if (goodOrb) ChallengerScore *= perfect ? PrefixBalance.CHALLENGER_GOOD_ORB_PERFECT_HIT_MULTIPLIER : PrefixBalance.CHALLENGER_GOOD_ORB_HIT_MULTIPLIER;

        switch (type)
        {
            case ChallengerOrbType.Green:
                ChallengerDefenseMultiplier += PrefixBalance.CHALLENGER_GREEN_ORB_DEFENSE;
                ChallengerDamageMultiplier += PrefixBalance.CHALLENGER_GREEN_ORB_DAMAGE;
                break;
            case ChallengerOrbType.Blue:
                ChallengerProjVelMultiplier += PrefixBalance.CHALLENGER_BLUE_ORB_VELOCITY;
                ChallengerCritMultiplier += PrefixBalance.CHALLENGER_BLUE_ORB_CRIT;
                break;
            case ChallengerOrbType.Yellow:
                ChallengerDefenseMultiplier += PrefixBalance.CHALLENGER_GREEN_ORB_DEFENSE;
                ChallengerDamageMultiplier += PrefixBalance.CHALLENGER_GREEN_ORB_DAMAGE;
                ChallengerProjVelMultiplier += PrefixBalance.CHALLENGER_BLUE_ORB_VELOCITY;
                ChallengerCritMultiplier += PrefixBalance.CHALLENGER_BLUE_ORB_CRIT;
                Projectile.NewProjectile(new EntitySource_OnHit(Main.player[projectile.owner], victim), victim.position.X, victim.position.Y, 0f, 0f, ProjectileID.VampireHeal, 0, 0f, Main.myPlayer,
                    Main.myPlayer, PrefixBalance.CHALLENGER_YELLOW_ORB_HEAL);
                break;
            case ChallengerOrbType.Red:
                ResetChallengerStats();
                Player.Hurt(MiscStuff.GetChallengerRedOrbDeath(Player.name), PrefixBalance.CHALLENGER_RED_ORB_DAMAGE, 0, armorPenetration: float.MaxValue);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        if (Main.netMode is NetmodeID.SinglePlayer) return;

        SyncPlayer(-1, Main.myPlayer, false);
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        ModPacket packet = Mod.GetPacket();
        packet.Write((byte)MessageType.ChallengerScore);
        packet.Write((byte)Player.whoAmI);
        packet.Write(ChallengerScore);
        packet.Send(toWho, fromWho);
    }

    public override void SendClientChanges(ModPlayer clientPlayer)
    {
        ChallengerModPlayer clone = (ChallengerModPlayer)clientPlayer;

        if (Math.Abs(ChallengerScore - clone.ChallengerScore) > 0.01f) SyncPlayer(-1, Main.myPlayer, false);
    }

    public override void CopyClientState(ModPlayer targetCopy)
    {
        ChallengerModPlayer clone = (ChallengerModPlayer)targetCopy;

        clone.ChallengerScore = ChallengerScore;
    }

    public void ReceivePlayerSync(BinaryReader reader)
    {
        ChallengerScore = reader.ReadSingle();
    }


    public override void OnEnterWorld()
    {
        ResetChallengerStats();
    }

    private void ResetChallengerStats()
    {
        ChallengerScore = 1f;
        ChallengerProjVelMultiplier = 1f;
        ChallengerDamageMultiplier = 1f;
        ChallengerDefenseMultiplier = 1f;
        ChallengerCritMultiplier = 1f;
        ChallengerModeActive = false;
    }

    private void MultiplyChallengerStats(float multiplier)
    {
        ChallengerProjVelMultiplier -= ChallengerProjVelMultiplier - 1 - (ChallengerProjVelMultiplier - 1) * multiplier;
        ChallengerDamageMultiplier -= ChallengerDamageMultiplier - 1 - (ChallengerDamageMultiplier - 1) * multiplier;
        ChallengerDefenseMultiplier -= ChallengerDefenseMultiplier - 1 - (ChallengerDefenseMultiplier - 1) * multiplier;
        ChallengerCritMultiplier -= ChallengerCritMultiplier - 1 - (ChallengerCritMultiplier - 1) * multiplier;
    }

    private void ChallengerTick()
    {
        if (ChallengerModeActive && ChallengerScore <= PrefixBalance.CHALLENGER_DEACTIVATION_SCORE_THRESHOLD) ChallengerModeActive = false;

        if (!ChallengerModeActive && ChallengerScore >= PrefixBalance.CHALLENGER_ACTIVATION_SCORE_THRESHOLD) ChallengerModeActive = true;

        if (ChallengerModeActive) AddChallengerDefense();

        bool challengerWeaponHeld = Player.HeldItem.HasPrefix(ModContent.PrefixType<PrefixChallenger>());

        if (!challengerWeaponHeld && !ChallengerModeActive) return;

        SpawnOrb();
    }


    private void AddChallengerDefense()
    {
        Player.statDefense *= GetChallengerScalar(ChallengerDefenseMultiplier);
    }

    private float GetChallengerScalar(float targetMultiplier)
    {
        float newMultiplier = targetMultiplier * ChallengerScore;
        return newMultiplier;
    }

    private void SpawnOrb()
    {
        ticksUntilChallengerOrbSpawn--;

        if (ticksUntilChallengerOrbSpawn > 0) return;

        Vector2 playerPos = Player.Center;
        Vector2 mousePos = Main.MouseWorld;
        Vector2 targetSpawnPos = GenerateOrbSpawnPoint(playerPos, mousePos, 90, 50f, 100f);
        if (Main.netMode is NetmodeID.SinglePlayer or NetmodeID.Server)
        {
            NPC challengerOrb = NPC.NewNPCDirect(new EntitySource_Misc("ChallengerOrbSpawn"), (int)targetSpawnPos.X, (int)targetSpawnPos.Y, ModContent.NPCType<ChallengerOrb>());
            ChallengerGlobalNPC prefixGlobalNpc = challengerOrb.GetGlobalNPC<ChallengerGlobalNPC>();
            prefixGlobalNpc.ChallengerOwner = Player.whoAmI;
        }

        ticksUntilChallengerOrbSpawn = PrefixBalance.GetChallengerSpawnRate(ChallengerScore);
    }

    private static Vector2 GenerateOrbSpawnPoint(Vector2 playerPos, Vector2 mousePos, float degrees, float minDistance, float maxDistance)
    {
        Vector2 direction = mousePos - playerPos;
        direction.Normalize();

        float baseAngle = (float)Math.Atan2(direction.Y, direction.X);
        baseAngle += MathHelper.ToRadians(180f);

        float angleOffset = MathHelper.ToRadians(degrees);

        float randomAngle = baseAngle + Main.rand.NextFloat(-angleOffset, angleOffset);

        float distance = Main.rand.NextFloat(minDistance, maxDistance);

        Vector2 offset = new Vector2((float)Math.Cos(randomAngle), (float)Math.Sin(randomAngle)) * distance;

        return playerPos + offset;
    }

    public override void ModifyWeaponCrit(Item item, ref float crit)
    {
        if (ChallengerModeActive) crit *= GetChallengerScalar(ChallengerCritMultiplier);
    }

    public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
    {
        if (item != Main.LocalPlayer.HeldItem) return;

        if (ChallengerModeActive) damage *= GetChallengerScalar(ChallengerDamageMultiplier);
    }

    public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (ChallengerModeActive) velocity *= GetChallengerScalar(ChallengerProjVelMultiplier);
    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        if (!ChallengerModeActive && !Player.HeldItem.HasPrefix(ModContent.PrefixType<PrefixChallenger>())) return;

        Vector2 targetPos = Player.Center - Main.screenPosition + new Vector2(0f, Player.gfxOffY);
        string targetText = $"Score: {MathF.Round(ChallengerScore, 2)}";
        float textScale = 0.8f;
        float textOffset = FontAssets.MouseText.Value.MeasureString(targetText).X / 2 * textScale;
        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, targetText, targetPos + new Vector2(-textOffset, -40), Color.WhiteSmoke, 0f, Vector2.Zero,
            new Vector2(textScale, textScale));

        //draw current bonuses to the right of player?
    }

    public override void PostUpdateEquips()
    {
        ChallengerTick();
    }
}