using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.Phantom;

public class PhantomArmorProjectile : ModProjectile
{
    public static Projectile CreatingPhantomProjectile;
    private float currentOrbitAngle;
    private float currentOrbitDistance;

    private int lastItemType = -1;
    private float maxOrbitDistance = 120f;
    private float orbitDistanceIncreasingSpeed = 2f;

    private float orbitSpeed = 5f;
    public Player phantom;
    public override string Texture => $"{Mod.Name}/Assets/Textures/Projectiles/SplinteringProjectile";

    private Player owner => Main.player[Projectile.owner];

    public override void SetDefaults()
    {
        Projectile.width = 20;
        Projectile.height = 20;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.DamageType = DamageClass.Generic;
        Projectile.timeLeft = int.MaxValue;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (phantom == null) return false;

        phantom.itemRotation = owner.itemRotation;
        phantom.direction = owner.direction;

        if (phantom.heldProj >= 0) phantom.itemAnimation = 0;

        Main.PlayerRenderer.DrawPlayer(Main.Camera, phantom, Projectile.position, 0f, phantom.fullRotationOrigin);
        return false;
    }

    public override void AI()
    {
        Projectile.Center = GetRotatedPoint(owner.Center, currentOrbitDistance, currentOrbitAngle);
        currentOrbitAngle += orbitSpeed;
        currentOrbitDistance += orbitDistanceIncreasingSpeed;
        currentOrbitDistance = Math.Clamp(currentOrbitDistance, 0f, maxOrbitDistance);

        phantom ??= (Player)owner.Clone();

        int currentItemType = owner.HeldItem.type;
        bool weaponChanged = currentItemType != lastItemType;

        if (weaponChanged || !owner.active || owner.dead)
        {
            if (phantom.heldProj >= 0 && Main.projectile[phantom.heldProj].active && Main.projectile[phantom.heldProj].owner == owner.whoAmI) Main.projectile[phantom.heldProj].Kill();

            phantom.heldProj = -1;
            phantom.itemAnimation = 0;
            phantom.itemTime = 0;
            phantom.reuseDelay = 0;
            phantom.releaseUseItem = true;

            lastItemType = currentItemType;
        }

        bool oldRelease = phantom.releaseUseItem;
        int oldItemAnim = phantom.itemAnimation;
        int oldItemTime = phantom.itemTime;
        int oldReuse = phantom.reuseDelay;
        int oldHeldProj = phantom.heldProj;
        int heldShoot = phantom.HeldItem.shoot;
        int oldProjCount = 0;

        if (heldShoot > 0 && heldShoot < phantom.ownedProjectileCounts.Length) oldProjCount = phantom.ownedProjectileCounts[heldShoot];

        phantom = (Player)owner.Clone();

        if (!weaponChanged)
        {
            phantom.releaseUseItem = oldRelease;
            phantom.itemAnimation = oldItemAnim;
            phantom.itemTime = oldItemTime;
            phantom.reuseDelay = oldReuse;
            phantom.heldProj = oldHeldProj;

            if (heldShoot > 0 && heldShoot < phantom.ownedProjectileCounts.Length) phantom.ownedProjectileCounts[heldShoot] = oldProjCount;
        }

        phantom.Center = Projectile.Center;
        phantom.position = Projectile.position;
        phantom.whoAmI = owner.whoAmI;
        phantom.controlUseItem = owner.controlUseItem;
        phantom.controlUseTile = owner.controlUseTile;
        phantom.itemLocation = Projectile.Center;

        Player realPlayer = Main.player[Projectile.owner];
        Main.player[Projectile.owner] = phantom;
        CreatingPhantomProjectile = Projectile;

        try
        {
            phantom.ItemCheck();

            if (owner.controlUseItem || phantom.heldProj < 0) return;

            Projectile p = Main.projectile[phantom.heldProj];
            if (!p.active || (p.ModProjectile == null && p.type != ProjectileID.Arkhalis)) return;

            if (!phantom.HeldItem.channel) return;

            p.Kill();
            phantom.heldProj = -1;
        }
        finally
        {
            Main.player[Projectile.owner] = realPlayer;
            CreatingPhantomProjectile = null;
        }
    }

    public void InitiateCloneDestruction()
    {
        Projectile.Kill();
    }

    public override void PostAI()
    {
        owner.AddBuff(ModContent.BuffType<VoidSealBuff>(), 2);
        owner.AddBuff(ModContent.BuffType<SoulBurnBuff>(), 1);
    }

    public void Initialize(float initialAngle)
    {
        currentOrbitAngle = initialAngle;
    }

    private static Vector2 GetRotatedPoint(Vector2 center, float distance, float angle)
    {
        float angleInRadians = angle * (float)Math.PI / 180f;
        return new Vector2(center.X + distance * (float)Math.Cos(angleInRadians), center.Y + distance * (float)Math.Sin(angleInRadians));
    }
}