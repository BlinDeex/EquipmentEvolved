using System;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Axe.Fortune;

public class FortuneGlobalItem : GlobalItem
{
    public override bool InstancePerEntity => true;

    /// <summary>
    ///     Is this item a fortune drop?
    /// </summary>
    public bool FortuneDrop { get; set; }

    public override void OnSpawn(Item item, IEntitySource source)
    {
        OnSpawnFortune(item, source);
    }

    public void OnSpawnFortune(Item item, IEntitySource source)
    {
        if (source is not EntitySource_TileBreak tileBreak) return;

        if (FortuneDrop) return;

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
                FortuneDrop = true;
            }
        }
    }

    private static Player GetNearbyFortunePlayer(Vector2 position)
    {
        for (int i = 0; i < Main.maxPlayers; i++)
        {
            Player p = Main.player[i];
            if (!p.active || p.dead) continue;

            if (p.GetModPlayer<FortuneModPlayer>().AxeFortune <= 0) continue;

            if (Vector2.Distance(p.Center, position) < 1000f) return p;
        }

        return null;
    }

    public override void HoldItem(Item item, Player player)
    {
        if (item.HasPrefix(ModContent.PrefixType<PrefixFortune>()) && player.itemAnimation > 0) player.GetModPlayer<FortuneModPlayer>().ActivateFortune();
    }
}