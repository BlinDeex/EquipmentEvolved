using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.Perceptive;

public class PerceptiveGlobalProjectile : GlobalProjectile
{
    public bool IsPerceptive;

    public int StoredItemCrit;
    public override bool InstancePerEntity => true;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source is not IEntitySource_WithStatsFromItem itemSource) return;

        if (!itemSource.Item.HasPrefix(ModContent.PrefixType<PrefixPerceptive>())) return;

        IsPerceptive = true;

        Player player = Main.player[projectile.owner];
        StoredItemCrit = player.GetWeaponCrit(itemSource.Item);
    }

    public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
    {
        if (!IsPerceptive) return;

        modifiers.ModifyHitInfo += (ref NPC.HitInfo info) =>
        {
            if (!info.Crit) return;

            int currentCrit = StoredItemCrit;
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