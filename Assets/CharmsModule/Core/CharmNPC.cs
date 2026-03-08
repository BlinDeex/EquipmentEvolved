using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.CharmsModule.Data;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.CharmsModule.Core;

public class CharmNPC : GlobalNPC
{
    public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone) // for some reason OnKill dont spawn any charms with a lot of npcs
    {
        if (npc.life <= 0) OnKilled(npc, player);
    }

    private void OnKilled(NPC npc, Player player)
    {
        if (!ModContent.GetInstance<PrefixConfig>().EnableCharms) return;

        if (Main.myPlayer != player.whoAmI) return;

        if (Main.netMode == NetmodeID.Server) return;

        if (CharmBalance.ExcludedNPCSFromCharmDrops.Contains(npc.type)) return;

        if (Main.netMode == NetmodeID.SinglePlayer)
        {
            List<(CharmRarity, CharmType)> rolledCharms = CharmsManager.RollForCharms(player.GetModPlayer<StatPlayer>().CharmLuckMul, npc);
            CharmsManager.SpawnCharms(rolledCharms, Main.LocalPlayer.whoAmI, npc);
            return;
        }

        ModPacket packet = Mod.GetPacket();
        packet.Write((byte)MessageType.CharmOnKilled);
        packet.Write(npc.Center.X);
        packet.Write(npc.Center.Y);
        packet.Write(npc.boss);
        packet.Send();
    }

    public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        if (npc.life <= 0) OnKilled(npc, Main.player[projectile.owner]);
    }
}