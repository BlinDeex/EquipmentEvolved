using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace EquipmentEvolved.Assets.ModPrefixes.Shared;

public class TimeStopGlobalProjectile : GlobalProjectile
{
    public override bool InstancePerEntity => true;

    public bool TimeStopActive { get; set; }
    public int TimeStopTicks { get; set; }

    public void ActivateTimeStop(int ticks, Projectile proj)
    {
        if (Main.projHook[proj.type]) return;

        TimeStopActive = true;
        TimeStopTicks = ticks * (1 + proj.extraUpdates);

        NetMessage.SendData(MessageID.SyncProjectile, number: proj.identity);
    }

    public override bool ShouldUpdatePosition(Projectile projectile)
    {
        if (TimeStopActive) return false;

        return base.ShouldUpdatePosition(projectile);
    }

    public override bool CanHitPlayer(Projectile projectile, Player target)
    {
        if (TimeStopActive) return false;

        return base.CanHitPlayer(projectile, target);
    }

    public override bool? CanHitNPC(Projectile projectile, NPC target)
    {
        if (TimeStopActive) return false;

        return base.CanHitNPC(projectile, target);
    }

    public override bool PreAI(Projectile projectile)
    {
        if (!TimeStopActive) return base.PreAI(projectile);

        projectile.timeLeft++;
        projectile.frameCounter--;

        TimeStopTicks--;

        if (TimeStopTicks > 0) return false;
        
        TimeStopActive = false;
        projectile.netUpdate = true;

        return false;
    }
    
    public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
    {
        bitWriter.WriteBit(TimeStopActive);
        binaryWriter.Write(TimeStopTicks);
    }

    public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
    {
        TimeStopActive = bitReader.ReadBit();
        TimeStopTicks = binaryReader.ReadInt32();
    }
}