using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Core;
using EquipmentEvolved.Assets.ModPrefixes.Shared;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.Chrono;

public class ChronoArmorPlayer : ModPlayer
{
    private static SoundStyle soundEffect = new($"{nameof(EquipmentEvolved)}/Assets/Sounds/TimeStop");

    public bool ChronoSetBonus;
    public int ChronoPiecesEquipped { get; set; }

    public override void PostUpdateEquips()
    {
        ChronoSetBonus = ChronoPiecesEquipped == 3;
        if (ChronoSetBonus) Player.GetModPlayer<ArmorAbilityPlayer>().SetArmorAbility(StopTimeAbility);
    }

    public override void ResetEffects()
    {
        ChronoPiecesEquipped = 0;
    }

    private int StopTimeAbility()
    {
        if (Main.dedServ)
        {
            ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral($"{nameof(StopTimeAbility)} somehow got called on server"), Color.Red);
            return PrefixBalance.CHRONO_ABILITY_COOLDOWN;
        }

        SoundEngine.PlaySound(soundEffect);

        if (Main.netMode is NetmodeID.MultiplayerClient)
        {
            SendTimeStopPacket(Player);
            return PrefixBalance.CHRONO_ABILITY_COOLDOWN;
        }

        List<NPC> npcsInRange = [];
        List<Projectile> projInRange = [];
        Vector2 playerPos = Player.Center;
        float maxDist = PrefixBalance.CHRONO_ABILITY_RANGE;

        foreach (NPC npc in Main.ActiveNPCs)
        {
            if (npc.friendly) continue;

            float range = Vector2.DistanceSquared(playerPos, npc.Center);
            if (range > maxDist) continue;

            npcsInRange.Add(npc);
        }

        foreach (Projectile proj in Main.ActiveProjectiles)
        {
            if (proj.friendly) continue;

            float range = Vector2.DistanceSquared(playerPos, proj.Center);
            if (range > maxDist) continue;

            projInRange.Add(proj);
        }

        foreach (NPC npc in npcsInRange)
        {
            npc.GetGlobalNPC<TimeStopGlobalNPC>().TimeStop(PrefixBalance.CHRONO_ABILITY_LENGTH, npc);
        }

        foreach (Projectile proj in projInRange)
        {
            proj.GetGlobalProjectile<TimeStopGlobalProjectile>().ActivateTimeStop(PrefixBalance.CHRONO_ABILITY_LENGTH, proj);
        }

        return PrefixBalance.CHRONO_ABILITY_COOLDOWN;
    }

    public static void SendTimeStopPacket(Player playerWhoActivated, int ignoreClient = -1)
    {
        ModPacket packet = EquipmentEvolved.Instance.GetPacket();
        packet.Write((byte)MessageType.TimeStop);
        packet.Write((byte)playerWhoActivated.whoAmI);
        packet.Send(ignoreClient: ignoreClient);
    }

    public static void PacketTimeStop(int playerWhoActivated)
    {
        Player player = Main.player[playerWhoActivated];

        List<NPC> NPCsInRange = [];
        List<Projectile> projInRange = [];
        Vector2 playerPos = player.Center;
        float maxDist = PrefixBalance.CHRONO_ABILITY_RANGE;

        foreach (NPC npc in Main.ActiveNPCs)
        {
            if (npc.friendly) continue;

            float range = Vector2.DistanceSquared(playerPos, npc.Center);
            if (range > maxDist) continue;

            NPCsInRange.Add(npc);
        }

        foreach (Projectile proj in Main.ActiveProjectiles)
        {
            if (proj.friendly) continue;

            float range = Vector2.DistanceSquared(playerPos, proj.Center);
            if (range > maxDist) continue;

            projInRange.Add(proj);
        }

        SoundEngine.PlaySound(soundEffect);


        foreach (NPC npc in NPCsInRange)
        {
            npc.GetGlobalNPC<TimeStopGlobalNPC>().TimeStop(PrefixBalance.CHRONO_ABILITY_LENGTH, npc);
        }

        foreach (Projectile proj in projInRange)
        {
            proj.GetGlobalProjectile<TimeStopGlobalProjectile>().ActivateTimeStop(PrefixBalance.CHRONO_ABILITY_LENGTH, proj);
        }
    }
}