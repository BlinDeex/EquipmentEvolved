using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPlayers;
using EquipmentEvolved.Assets.ModPrefixes.Melee;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Globals.Items;

public class GlobalMeleePrefix : GlobalItem
{
    public override bool CanUseItem(Item item, Player player)
    {
        if (item.prefix == 0) return base.CanUseItem(item, player);

        if (item.prefix == ModContent.PrefixType<PrefixArcaneInfused>())
        {
            bool usedMana = player.CheckMana(10, true);
            player.manaRegenDelay = 300;
            return usedMana;
        }

        return true;
    }

    public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
    {
        PrefixPlayer prefixPlayer = player.GetModPlayer<PrefixPlayer>();
        if (player.HasBuff(BuffID.ManaSickness) && item.prefix == ModContent.PrefixType<PrefixArcaneInfused>())
        {
            int index = player.FindBuffIndex(BuffID.ManaSickness);
            int timeLeft = player.buffTime[index];
            float targetReduction = timeLeft * 0.1f;
            damage.Flat = 1f - targetReduction;
        }

        if (item.prefix == ModContent.PrefixType<PrefixUntouchable>())
            damage *= prefixPlayer.UntouchableDamageIncrease + 1;
    }

    public override bool? CanAutoReuseItem(Item item, Player player)
    {
        return item.prefix == ModContent.PrefixType<PrefixUltraLight>() ? true : base.CanAutoReuseItem(item, player);
    }

    public override void ModifyHitNPC(Item item, Player player, NPC target, ref NPC.HitModifiers modifiers)
    {
        if (item.prefix == ModContent.PrefixType<PrefixPerceptive>())
        {
            PrefixPerceptive(item, player, target, ref modifiers);
        }
    }

    private void PrefixPerceptive(Item item, Player player, NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.ModifyHitInfo += (ref NPC.HitInfo info) =>
        {
            if (!info.Crit) return;

            int currentCrit = player.GetWeaponCrit(item);
            if (currentCrit <= 100) return;

            // --- Calc Logic ---
            float damageMult = 1f; 
            int critToCheck = currentCrit - 100;
            int tierReached = 0;
            const int MAX_TIER = 6;

            while (critToCheck > 0 && tierReached < MAX_TIER)
            {
                if (Main.rand.Next(0, 100) < critToCheck)
                {
                    damageMult += (tierReached == 0) ? 1f : 0.5f;
                    tierReached++;
                }
                else break;
                critToCheck -= 100;
            }
            // ------------------

            if (tierReached > 0)
            {
                info.Damage = (int)(info.Damage * damageMult);
                info.HideCombatText = true;

                if (Main.netMode != NetmodeID.Server)
                {
                    // 1. Show Local
                    WeaponUtils.SpawnPerceptiveText(target.Center, info.Damage, tierReached);
                    
                    // 2. Send Packet to others
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        ModPacket packet = Mod.GetPacket();
                        packet.Write((byte)MessageType.PerceptiveCritEffect);
                        packet.Write(target.Center.X);
                        packet.Write(target.Center.Y);
                        packet.Write(info.Damage);
                        packet.Write((byte)tierReached);
                        packet.Send();
                    }
                }
            }
        };
    }
}