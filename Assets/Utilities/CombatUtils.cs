using System;
using System.Collections.Generic;
using System.Linq;
using EquipmentEvolved.Assets.Balance;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace EquipmentEvolved.Assets.Utilities;

public static class CombatUtils
{
    public static void Lifesteal(Entity victim, Vector2 hitPosition, int healAmount, int playerWhoAmI)
    {
        if (victim is NPC { type: NPCID.TargetDummy } && !PrefixBalance.DEV_MODE) return;

        Projectile.NewProjectile(new EntitySource_OnHit(Main.player[playerWhoAmI], victim), hitPosition.X, hitPosition.Y, 0f, 0f, ProjectileID.VampireHeal, 0, 0f, playerWhoAmI, playerWhoAmI,
            healAmount);
    }

    public static void Lifesteal(Entity victim, Vector2 hitPosition, int healAmount, Player player)
    {
        Lifesteal(victim, hitPosition, healAmount, player.whoAmI);
    }

    public static void DropCoins(float value, Entity entityToDropCoinsFrom)
    {
        if (entityToDropCoinsFrom is NPC { type: NPCID.TargetDummy } && !PrefixBalance.DEV_MODE) return;

        bool isNegative = value < 0;
        Player player = null;
        if (isNegative) player = (Player)entityToDropCoinsFrom;

        float absValue = Math.Abs(value);
        int platinumCoins = (int)(absValue / 1000000);
        absValue -= platinumCoins * 1000000;

        int goldCoins = (int)(absValue / 10000);
        absValue -= goldCoins * 10000;

        int silverCoins = (int)(absValue / 100);
        absValue -= silverCoins * 100;

        int copperCoins = (int)absValue;
        
        if (platinumCoins > 0) DropCoinStack(entityToDropCoinsFrom, ItemID.PlatinumCoin, platinumCoins, isNegative, player);
        if (goldCoins > 0) DropCoinStack(entityToDropCoinsFrom, ItemID.GoldCoin, goldCoins, isNegative, player);
        if (silverCoins > 0) DropCoinStack(entityToDropCoinsFrom, ItemID.SilverCoin, silverCoins, isNegative, player);
        if (copperCoins > 0) DropCoinStack(entityToDropCoinsFrom, ItemID.CopperCoin, copperCoins, isNegative, player);
    }

    private static void DropCoinStack(Entity entity, int coinType, int stack, bool isNegative, Player player)
    {
        if (isNegative && player != null)
        {
            for (int i = 0; i < stack; i++) player.BuyItem(coinType);
        }
        
        int itemDropID = Item.NewItem(new EntitySource_Loot(entity, $"{nameof(EquipmentEvolved)} DropCoins"), 
            (int)entity.position.X, (int)entity.position.Y, entity.width, entity.height, coinType, stack);
        
        Item itemDrop = Main.item[itemDropID];
        itemDrop.velocity.Y = Main.rand.Next(-20, 1) * 0.2f;
        itemDrop.velocity.X = Main.rand.Next(-20, 21) * 0.2f;
        itemDrop.noGrabDelay = 100;
        itemDrop.newAndShiny = true;

        if (Main.netMode == NetmodeID.MultiplayerClient) NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemDropID);
    }

    /// <param name="velocity">root projectile triple shot originates from velocity</param>
    /// <param name="degrees">degrees</param>
    /// <param name="variation">variation to the degrees per projectile</param>
    /// <returns>Two other projectile velocities rotated by degrees</returns>
    public static (Vector2, Vector2) TripleShotRotatedVelocities(Vector2 velocity, float degrees, float variation)
    {
        float radians1 = MathF.PI / 180 * (degrees + Main.rand.NextFloat(-variation, variation));
        float radians2 = MathF.PI / 180 * (degrees + Main.rand.NextFloat(-variation, variation));

        Vector2 rotatedVelocityPositive = UtilMethods.RotateVector(velocity, radians1);
        Vector2 rotatedVelocityNegative = UtilMethods.RotateVector(velocity, -radians2);

        return (rotatedVelocityPositive, rotatedVelocityNegative);
    }

    public static bool TryFindSegments(NPC npc, out List<NPC> segments)
    {
        segments = [];
        int targetNpcRealLife = npc.realLife;
        if (targetNpcRealLife == -1) return false;

        foreach (NPC testNpc in Main.ActiveNPCs)
        {
            int testNpcRealLife = testNpc.realLife;

            if (testNpcRealLife != targetNpcRealLife) continue;

            segments.Add(testNpc);
        }

        return segments.Count > 0;
        //segments.Add(Main.npc[npc.realLife]);
    }
}