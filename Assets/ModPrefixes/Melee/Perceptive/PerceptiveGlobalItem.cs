using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.Perceptive;

public class PerceptiveGlobalItem : GlobalItem
{
    public override bool InstancePerEntity => true;

    public override void ModifyHitNPC(Item item, Player player, NPC target, ref NPC.HitModifiers modifiers)
    {
        if (!item.HasPrefix(ModContent.PrefixType<PrefixPerceptive>())) return;

        modifiers.ModifyHitInfo += (ref NPC.HitInfo info) =>
        {
            if (!info.Crit) return;

            int currentCrit = player.GetWeaponCrit(item);
            if (currentCrit <= 100) return;

            float damageMult = 1f;
            int critToCheck = currentCrit - 100;
            int tierReached = 0;
            int MAX_TIER = PrefixBalance.PERCEPTIVE_MAX_TIER;

            while (critToCheck > 0 && tierReached < MAX_TIER)
            {
                if (Main.rand.Next(0, 100) < critToCheck)
                {
                    damageMult += tierReached == 0 ? 1f : 0.5f;
                    tierReached++;
                }
                else
                    break;

                critToCheck -= 100;
            }

            if (tierReached <= 0) return;

            info.Damage = (int)(info.Damage * damageMult);
            info.HideCombatText = true;

            if (Main.netMode == NetmodeID.Server) return;

            WeaponUtils.SpawnPerceptiveText(target.Center, info.Damage, tierReached);

            if (Main.netMode != NetmodeID.MultiplayerClient) return;

            ModPacket packet = ModContent.GetInstance<EquipmentEvolved>().GetPacket();
            packet.Write((byte)MessageType.PerceptiveCritEffect);
            packet.Write(target.Center.X);
            packet.Write(target.Center.Y);
            packet.Write(info.Damage);
            packet.Write((byte)tierReached);
            packet.Send();
        };
    }
}