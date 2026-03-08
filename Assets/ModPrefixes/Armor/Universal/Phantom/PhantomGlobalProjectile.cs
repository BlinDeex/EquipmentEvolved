using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.Phantom;

public class PhantomGlobalProjectile : GlobalProjectile
{
    private Player _tempRealPlayer;

    public Projectile PhantomParent;
    public override bool InstancePerEntity => true;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (PhantomArmorProjectile.CreatingPhantomProjectile == null) return;

        PhantomParent = PhantomArmorProjectile.CreatingPhantomProjectile;
        if (PhantomParent.ModProjectile is PhantomArmorProjectile { phantom: not null } pap) projectile.Center = pap.phantom.Center;
    }

    public override bool PreAI(Projectile projectile)
    {
        if (PhantomParent != null && PhantomParent.active && PhantomParent.ModProjectile is PhantomArmorProjectile pap)
        {
            if (projectile.owner == Main.myPlayer)
            {
                if (_tempRealPlayer == null)
                {
                    _tempRealPlayer = Main.player[projectile.owner];
                    Main.player[projectile.owner] = pap.phantom;
                }
            }
        }

        return base.PreAI(projectile);
    }

    public override void PostAI(Projectile projectile)
    {
        if (_tempRealPlayer != null)
        {
            Main.player[projectile.owner] = _tempRealPlayer;
            _tempRealPlayer = null;
        }

        if (PhantomParent != null && PhantomParent.active && PhantomParent.ModProjectile is PhantomArmorProjectile pap)
            if (pap.phantom != null)
                projectile.Center = pap.phantom.Center + (projectile.Center - pap.phantom.MountedCenter);
    }

    public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
    {
        if (PhantomParent != null && PhantomParent.active)
            binaryWriter.Write((short)PhantomParent.identity);
        else
            binaryWriter.Write((short)-1);
    }

    public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
    {
        short parentIdentity = binaryReader.ReadInt16();

        if (parentIdentity > -1)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == projectile.owner && Main.projectile[i].identity == parentIdentity)
                {
                    PhantomParent = Main.projectile[i];
                    break;
                }
            }
        }
    }
}