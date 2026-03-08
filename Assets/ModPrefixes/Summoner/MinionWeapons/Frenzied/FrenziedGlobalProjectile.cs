using System.IO;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace EquipmentEvolved.Assets.ModPrefixes.Summoner.MinionWeapons.Frenzied;

public class FrenziedGlobalProjectile : GlobalProjectile
{
    public override bool InstancePerEntity => true;
    public bool Frenzied { get; set; }


    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        HandleFrenziedSpawn(projectile, source);
    }

    private void HandleFrenziedSpawn(Projectile projectile, IEntitySource source)
    {
        if (source is not IEntitySource_WithStatsFromItem itemSource || !itemSource.Item.HasPrefix(ModContent.PrefixType<PrefixFrenzied>())) return;
        
        Frenzied = true;
        if (Main.netMode != NetmodeID.SinglePlayer) projectile.netUpdate = true;
    }

    public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
    {
        bitWriter.WriteBit(Frenzied);
    }

    public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
    {
        Frenzied = bitReader.ReadBit();
    }
}