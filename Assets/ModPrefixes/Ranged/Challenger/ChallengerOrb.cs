using System;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Utilities;
using EquipmentEvolved.Assets.Utilities.Spritebatch;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Challenger;

public enum ChallengerOrbType
{
    Green = 0,
    Blue = 1,
    Yellow = 2,
    Red = 3
}

public class ChallengerOrb : ModNPC
{
    private const int FRAME_HEIGHT = 24;
    private const int FRAME_WIDTH = 24;
    private const int FRAME_COUNT = 56;
    private const int PERFECT_HIT_FRAME_START = 20;
    private const int PERFECT_HIT_FRAME_END = 38;

    private const string GOOD_ORB_HIT_SOUND = $"{nameof(EquipmentEvolved)}/Assets/Sounds/OrbHitGood";
    private const string BAD_ORB_HIT_SOUND = $"{nameof(EquipmentEvolved)}/Assets/Sounds/OrbHitBad";

    private readonly SoundStyle badOrbSound = new(BAD_ORB_HIT_SOUND)
    {
        SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest,
        MaxInstances = 10
    };

    private readonly SoundStyle goodOrbSound = new(GOOD_ORB_HIT_SOUND)
    {
        SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest,
        MaxInstances = 10
    };

    public ref float AI0 => ref NPC.ai[0];
    public ref float OrbSpeedMultiplier => ref NPC.ai[1];
    public ref float OrbType => ref NPC.ai[2];

    public ref float IsPerfectTimeToHit => ref NPC.ai[3];

    private ref float orbGravityMultiplier => ref NPC.localAI[0];

    public override void SetDefaults()
    {
        Main.npcFrameCount[Type] = FRAME_COUNT;
        NPC.width = 16;
        NPC.height = 16;
        NPC.aiStyle = -1;
        NPC.noTileCollide = true;

        NPC.damage = 0;
        NPC.defense = 0;
        NPC.lifeMax = 1;
        NPC.value = 0;
    }

    public override void OnSpawn(IEntitySource source)
    {
        ChallengerGlobalNPC prefixGlobalNpc = NPC.GetGlobalNPC<ChallengerGlobalNPC>();
        Player owner = Main.player[prefixGlobalNpc.ChallengerOwner];
        int challengerOrbOwner = prefixGlobalNpc.ChallengerOwner;

        if (challengerOrbOwner == -1)
        {
            Main.NewText("Challenger orb is not designed to be spawned by any other means than weapons with challenger reforge!", Color.Red);
            NPC.active = false;
            return;
        }

        float currentScore = owner.GetModPlayer<ChallengerModPlayer>().ChallengerScore;
        OrbSpeedMultiplier = PrefixBalance.CHALLENGER_BASELINE_ORB_SPEED + currentScore * PrefixBalance.CHALLENGER_ORB_SPEED_SCORE_PERCENTAGE_MULTIPLIER;
        OrbType = Main.rand.Next(0, 4); // max exclusive
        orbGravityMultiplier = Main.rand.NextBool() ? 1 : -1;

        NPC.netUpdate = true;
    }


    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D texture = TextureAssets.Npc[Type].Value;

        int frameHeight = texture.Height / Main.npcFrameCount[Type];
        int startY = frameHeight * (int)NPC.frameCounter;
        Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);

        Vector2 origin = sourceRectangle.Size() / 2f;

        Color targetColor = NPC.GetAlpha(drawColor);

        SpriteBatchSnapshot snapshot = spriteBatch.CaptureSnapshot();
        spriteBatch.End();
        Main.spriteBatch.Begin(snapshot with { BlendState = BlendState.Additive });

        Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY), sourceRectangle, targetColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None);

        spriteBatch.End();
        spriteBatch.Begin(snapshot);

        return false;
    }

    public override void AI()
    {
        AI0++;
        NPC.GravityMultiplier *= 0;
        NPC.frameCounter = AI0 * PrefixBalance.CHALLENGER_FRAME_RATE_MULTIPLIER;
        IsPerfectTimeToHit = NPC.frameCounter is >= PERFECT_HIT_FRAME_START and < PERFECT_HIT_FRAME_END ? 1 : 0;

        ChallengerGlobalNPC prefixGlobalNpc = NPC.GetGlobalNPC<ChallengerGlobalNPC>();
        Player owner = Main.player[prefixGlobalNpc.ChallengerOwner];
        int challengerOrbOwner = prefixGlobalNpc.ChallengerOwner;
        if (challengerOrbOwner == -1)
        {
            Main.NewText("Challenger orb is not designed to be spawned by any other means than weapons with challenger reforge!", Color.Red);
            NPC.active = false;
            return;
        }

        if (NPC.frameCounter > FRAME_COUNT)
        {
            OnMiss();
            NPC.active = false;
            return;
        }

        Vector2 ownerPos = owner.Center;
        MoveOrb(ownerPos);
        Effects();
    }

    private void Effects()
    {
        float spawnRate = 0.3f;
        if (IsPerfectTimeToHit > 0) spawnRate *= 5f;

        int guaranteedSpawns = (int)spawnRate;

        const float radius = 10f;
        for (int i = 0; i < guaranteedSpawns; i++)
        {
            SpawnDust(radius);
        }

        spawnRate -= guaranteedSpawns;
        float dice = Main.rand.NextFloat();
        if (dice > spawnRate) return;

        SpawnDust(radius);
    }

    private void OnHit(Projectile projectile)
    {
        ChallengerGlobalNPC prefixGlobalNpc = NPC.GetGlobalNPC<ChallengerGlobalNPC>();
        int challengerOrbOwner = prefixGlobalNpc.ChallengerOwner;
        Player targetPlayer = Main.player[challengerOrbOwner];
        if (!targetPlayer.active) return;

        //var redOrb = (ChallengerOrbType)(int)OrbType == ChallengerOrbType.Red;
        GeneralOrbHit(targetPlayer, projectile);
    }

    private void GeneralOrbHit(Player targetPlayer, Projectile projectile)
    {
        targetPlayer.GetModPlayer<ChallengerModPlayer>().OrbHit((ChallengerOrbType)(int)OrbType, IsPerfectTimeToHit > 0, NPC, projectile);
    }


    private void SpawnPopUpText(bool redOrb = false)
    {
        string text = IsPerfectTimeToHit > 0 ? "Perfect!" : "Good";
        if (redOrb) text = "Red orb hit, score was lost!";

        AdvancedPopupRequest request = new()
        {
            DurationInFrames = 60,
            Color = GetOrbColor((ChallengerOrbType)(int)OrbType),
            Text = text,
            Velocity = new Vector2(Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f))
        };
        PopupText.NewText(request, NPC.Center + new Vector2(0, 0));
    }

    private void OnMiss()
    {
        ChallengerGlobalNPC prefixGlobalNpc = NPC.GetGlobalNPC<ChallengerGlobalNPC>();
        int challengerOrbOwner = prefixGlobalNpc.ChallengerOwner;
        Player targetPlayer = Main.player[challengerOrbOwner];

        if (!targetPlayer.active) return;

        if ((ChallengerOrbType)(int)OrbType == ChallengerOrbType.Red) return;

        targetPlayer.GetModPlayer<ChallengerModPlayer>().GoodOrbMiss();
    }

    private void SpawnDust(float radius)
    {
        Vector2 pos = UtilMethods.RandomPointInCircle(NPC.Center.X, NPC.Center.Y, radius, Main.rand);
        int dustID = Dust.NewDust(pos, 1, 1, ModContent.DustType<ChallengerDust>(), newColor: GetOrbDustColor((ChallengerOrbType)(int)OrbType));
        Main.dust[dustID].customData = (ChallengerOrbType)(int)OrbType;
    }

    private void HitDust()
    {
        int nodeCount = Main.rand.Next(4, 9);

        for (int i = 0; i < nodeCount; i++)
        {
            Vector2 primaryDirection = Main.rand.NextVector2CircularEdge(1f, 1f);
            int dustCount = Main.rand.Next(5, 11);

            for (int j = 0; j < dustCount; j++)
            {
                Vector2 variation = primaryDirection.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f));

                int dustID = Dust.NewDust(NPC.Center, 1, 1, ModContent.DustType<ChallengerDust>(), newColor: GetOrbDustColor((ChallengerOrbType)(int)OrbType));
                Dust dust = Main.dust[dustID];
                dust.velocity = variation * Main.rand.NextFloat(20f, 40f);
                Main.dust[dustID].customData = (ChallengerOrbType)(int)OrbType;
            }
        }
    }

    public static Color GetOrbDustColor(ChallengerOrbType orbType)
    {
        return orbType switch
        {
            ChallengerOrbType.Green => new Color(0, 255, 0, 0f),
            ChallengerOrbType.Blue => new Color(0, 0, 255, 0f),
            ChallengerOrbType.Yellow => new Color(255, 255, 0, 0f),
            ChallengerOrbType.Red => new Color(255, 0, 0, 0f),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static Color GetOrbColor(ChallengerOrbType orbType)
    {
        return orbType switch
        {
            ChallengerOrbType.Green => new Color(0, 255, 0, 1f),
            ChallengerOrbType.Blue => new Color(0, 0, 255, 1f),
            ChallengerOrbType.Yellow => new Color(255, 255, 0, 1f),
            ChallengerOrbType.Red => new Color(255, 0, 0, 1f),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void MoveOrb(Vector2 ownerPos)
    {
        float moveAwaySpeed = 1f * OrbSpeedMultiplier;
        float wiggleSpeed = 1.6f * OrbSpeedMultiplier;

        Vector2 directionAwayFromPlayer = NPC.Center - ownerPos;
        directionAwayFromPlayer.Normalize();

        Vector2 moveAwayVelocity = directionAwayFromPlayer * moveAwaySpeed;

        float wiggleX = (Main.rand.NextFloat() - 0.5f) * wiggleSpeed;
        float wiggleY = (Main.rand.NextFloat() - 0.5f) * wiggleSpeed;
        Vector2 wiggleVelocity = new(wiggleX, wiggleY);

        Vector2 totalVelocity = moveAwayVelocity + wiggleVelocity;

        if (NPC.frameCounter >= FRAME_COUNT / 2f)
        {
            float upwardSpeedMultiplier = (float)(NPC.frameCounter - FRAME_COUNT / 2f) / (FRAME_COUNT / 2f);
            upwardSpeedMultiplier *= orbGravityMultiplier;
            totalVelocity.Y -= 2f * upwardSpeedMultiplier;
        }

        NPC.velocity = totalVelocity;
    }

    public override bool? CanBeHitByProjectile(Projectile projectile)
    {
        if (projectile.owner != Main.myPlayer) return false;
        if (projectile.TryGetGlobalProjectile(out ChallengerGlobalProjectile challengerProj))
        {
            if (!challengerProj.IsChallenger) return false;
        }
        else
            return false;

        return true;
    }

    public override bool? CanBeHitByItem(Player player, Item item)
    {
        return false;
    }

    public override bool CanBeHitByNPC(NPC attacker)
    {
        return false;
    }

    public override Color? GetAlpha(Color drawColor)
    {
        return GetOrbColor((ChallengerOrbType)(int)OrbType);
    }

    public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
    {
        modifiers.ModifyHitInfo += (ref NPC.HitInfo info) => { info.HideCombatText = true; };
    }

    public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        if (NPC.life > 0) return;

        OnHit(projectile);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life > 0) return;

        bool redOrb = (ChallengerOrbType)(int)OrbType == ChallengerOrbType.Red;
        SpawnPopUpText(redOrb);
        HitDust();
        SoundEngine.PlaySound(redOrb ? badOrbSound : goodOrbSound);
    }
}