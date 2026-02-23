using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace EquipmentEvolved.Assets.InstancedGlobalItems;

public class InstancedProjectilePrefix : GlobalProjectile
{
    public override bool InstancePerEntity => true;
    public Item ItemUsed { get; set; }
    public int GunPrefixType { get; set; }
    public int ItemCrit { get; set; }

    public bool TripleShootClone { get; set; }

    /// <summary>
    /// Used for minions by frenzied prefix
    /// </summary>
    public bool Frenzied { get; set; }
    
    public bool AdaptableSwapped { get; set; }

    public int AmmoTypeUsed { get; set; } = -1;
    
    public bool Tracer { get; set; }
    public List<Vector2> TracerPathPoints { get; set; } = [];
    
    public bool TimeStop { get; set; }
    public int TimeStopTicks { get; set; }
    
    public Player PhantomOwner;
    public Projectile PhantomParent;
    
    public bool Monarch { get; set; }
    public float MonarchStacks { get; set; } = 0;
    public bool MonarchChild { get; set; }
    public float MonarchDamageMult { get; set; } = 1f;
    
    public float BaseDamage { get; set; } // Stores the original base damage for calculations

    public void ActivateTimeStop(int ticks, Projectile proj)
    {
        if (Main.projHook[proj.type]) return;
        
        TimeStop = true;

        // FIX: Account for the base update (1) PLUS extra updates.
        // Formula: DesiredFrames * (1 + ExtraUpdates)
        TimeStopTicks = ticks * (1 + proj.extraUpdates); 

        NetMessage.SendData(MessageID.SyncProjectile, number: proj.identity);
    }

    public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
    {
        bitWriter.WriteBit(Frenzied);
        bitWriter.WriteBit(TimeStop);
        binaryWriter.Write(TimeStopTicks);

        // FIX: Sync the Phantom Parent Identity
        // If we have a parent, send its ID. If not, send -1.
        if (PhantomParent is { active: true })
        {
            binaryWriter.Write((short)PhantomParent.identity);
        }
        else
        {
            binaryWriter.Write((short)-1);
        }
        
        bitWriter.WriteBit(Monarch);
        binaryWriter.Write(MonarchStacks);
        bitWriter.WriteBit(MonarchChild);
        binaryWriter.Write(MonarchDamageMult);
    }

    public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
    {
        Frenzied = bitReader.ReadBit();
        TimeStop = bitReader.ReadBit();
        TimeStopTicks = binaryReader.ReadInt32();

        // FIX: Read Phantom Parent Identity and find the projectile
        short parentIdentity = binaryReader.ReadInt16();
        if (parentIdentity <= -1)
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
        
        Monarch = bitReader.ReadBit();
        MonarchStacks = binaryReader.ReadSingle();
        MonarchChild = bitReader.ReadBit();
        MonarchDamageMult = binaryReader.ReadSingle();
    }
}