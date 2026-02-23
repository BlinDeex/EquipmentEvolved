using System;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.InstancedGlobalItems;
using EquipmentEvolved.Assets.ModPlayers;
using EquipmentEvolved.Assets.ModPrefixes.Axe;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Globals.Items;

public class PrefixGlobalItem : GlobalItem
{
    // FIX 1: Use HoldItem instead of UseItem.
    // This runs every frame the item is held. We check if animation > 0 (swinging).
    public override void HoldItem(Item item, Player player)
    {
        if (item.prefix == ModContent.PrefixType<PrefixFortune>() && player.itemAnimation > 0)
        {
            player.GetModPlayer<ToolPlayer>().ActivateFortune();
        }
    }
    
    public override void OnSpawn(Item item, IEntitySource source)
    {
        OnSpawnFortune(item, source);
    }

    public void OnSpawnFortune(Item item, IEntitySource source)
    {
        if (source is not EntitySource_TileBreak tileBreak) return;

        InstancedGlobalItem instanced = item.GetGlobalItem<InstancedGlobalItem>();
        if (instanced.FortuneDrop) return;

        // FIX 2: Check distance from the ITEM position, not the tile position.
        Player fortunePlayer = GetNearbyFortunePlayer(item.Center);
        
        if (fortunePlayer != null)
        {
            if (Main.rand.NextFloat() <= PrefixBalance.FORTUNE_CHANCE_FOR_EXTRA_DROPS)
            {
                int max = PrefixBalance.FORTUNE_MAX_EXTRA_DROPS;
                int roll1 = Main.rand.Next(1, max + 1);
                int roll2 = Main.rand.Next(1, max + 1);
                int bonusStack = Math.Max(roll1, roll2);

                item.stack += bonusStack;
                instanced.FortuneDrop = true;
            }
        }
    }

    private Player GetNearbyFortunePlayer(Vector2 position)
    {
        for (int i = 0; i < Main.maxPlayers; i++)
        {
            Player p = Main.player[i];
            if (p.active && !p.dead)
            {
                if (p.GetModPlayer<ToolPlayer>().AxeFortune > 0)
                {
                    // FIX 3: Increased range from 200f (12 blocks) to 1000f (62 blocks).
                    // Trees can be very tall, and drops from the top were failing this check.
                    if (Vector2.Distance(p.Center, position) < 1000f)
                    {
                        return p;
                    }
                }
            }
        }
        return null;
    }

    public override bool CanEquipAccessory(Item item, Player player, int slot, bool modded)
    {
        return !item.IsArmor();
    }
}