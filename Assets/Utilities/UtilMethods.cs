using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using EquipmentEvolved.Assets.Misc;
using log4net;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace EquipmentEvolved.Assets.Utilities;

public static class UtilMethods
{
    public static string FormatNumber(float number)
    {
        return number switch
        {
            >= 1000000000000 => (number / 1000000000000f).ToString("0.#") + "T",
            >= 1000000000 => (number / 1000000000f).ToString("0.#") + "B",
            >= 1000000 => (number / 1000000f).ToString("0.#") + "M",
            >= 1000 => (number / 1000f).ToString("0.#") + "K",
            _ => number.ToString(CultureInfo.InvariantCulture)
        };
    }
    
    public static bool IsArmor(this Item item)
    {
        return IsHeadwear(item) || IsChestplate(item) || IsLeggings(item);
    }

    public static bool IsFishingRod(this Item item)
    {
        return item.fishingPole > 0;
    }

    public static bool IsHeadwear(this Item item) => item.headSlot != -1;

    public static bool IsChestplate(this Item item) => item.bodySlot != -1;

    public static bool IsLeggings(this Item item) => item.legSlot != -1;
    
    public static bool IsPickaxe(this Item item) => item.pick > 0;
    
    public static bool IsAxe(this Item item) => item.axe > 0;
    
    public static bool IsHammer(this Item item) => item.hammer > 0;
    
    public static bool IsWhip(this Item item) => item.DamageType == DamageClass.SummonMeleeSpeed && !Main.projPet[item.shoot];
    public static bool IsMinionWeapon(this Item item) => Main.projPet[item.shoot];
    
    public static bool IsMeleeWeapon(this Item item) => item.CountsAsClass(DamageClass.Melee) && !item.IsPickaxe() && !item.IsAxe() && !item.IsHammer();

    public static bool IsRangedWeapon(this Item item) => item.CountsAsClass(DamageClass.Ranged);

    public static bool IsMagicWeapon(this Item item) => item.CountsAsClass(DamageClass.Magic);
    
    public static void BroadcastOrNewText(string message, Color color)
    {
        if (Main.dedServ)
        {
            ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(message), color);
            return;
        }
        
        Main.NewText(message, color);
    }
    
    public static byte[] IntListToByteArray(List<int> intList)
    {
        byte[] byteArray = new byte[intList.Count * 4];

        for (int i = 0; i < intList.Count; i++)
        {
            byte[] bytes = BitConverter.GetBytes(intList[i]);
            Buffer.BlockCopy(bytes, 0, byteArray, i * 4, 4);
        }

        return byteArray;
    }
    
    public static Vector3 ColorToVector3(this Color color)
    {
        return new Vector3(color.R, color.G, color.B);
    }
    
    public static List<int> ByteArrayToIntList(byte[] byteArray)
    {
        List<int> intList = [];

        for (int i = 0; i < byteArray.Length; i += 4)
        {
            int value = BitConverter.ToInt32(byteArray, i);
            intList.Add(value);
        }

        return intList;
    }

    public static List<Point> GetConnectedTiles(int startX, int startY, int maxTiles, int type)
    {
        List<Point> connectedTiles = [];
        HashSet<(int x, int y)> visited = [];

        Point[] directions =
        [
            new(-1, 0), // left
            new(1, 0), // right
            new(0, -1), // up
            new(0, 1) // down
        ];

        Queue<Point> toExplore = new Queue<Point>();
        toExplore.Enqueue(new Point(startX, startY));
        visited.Add((startX, startY));

        while (toExplore.Count > 0 && connectedTiles.Count < maxTiles)
        {
            Point currentExplore = toExplore.Dequeue();
            connectedTiles.Add(new Point(currentExplore.X, currentExplore.Y));

            foreach (Point direction in directions)
            {
                int newX = currentExplore.X + direction.X;
                int newY = currentExplore.Y + direction.Y;

                if (!IsWithinBounds(newX, newY) || visited.Contains((newX, newY)) ||
                    Main.tile[newX, newY].TileType != type) continue;

                toExplore.Enqueue(new Point(newX, newY));
                visited.Add((newX, newY));
            }
        }

        return connectedTiles;
    }

    private static bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x < Main.maxTilesX && y >= 0 && y < Main.maxTilesY;
    }


    public static Vector2 RandomPointInCircle(float centerX, float centerY, float radius, UnifiedRandom random)
    {
        double angle = random.NextDouble() * 2 * Math.PI;

        double randomRadius = radius * Math.Sqrt(random.NextDouble());

        double x = centerX + randomRadius * Math.Cos(angle);
        double y = centerY + randomRadius * Math.Sin(angle);

        return new Vector2((float)x, (float)y);
    }

    public static Vector2 GetRandomPositionInRectangle(Rectangle rect, UnifiedRandom random)
    {
        int randomX = random.Next(rect.Left, rect.Right);
        int randomY = random.Next(rect.Top, rect.Bottom);
        return new Vector2(randomX, randomY);
    }

    public static TEnum ToggleFlag<TEnum>(this TEnum target, TEnum flag) where TEnum : unmanaged, Enum
    {
        int tVal = Unsafe.As<TEnum, int>(ref target);
        int fVal = Unsafe.As<TEnum, int>(ref flag);
        
        int result = tVal ^ fVal;
        
        return Unsafe.As<int, TEnum>(ref result);
    }

    public static Vector2 RotateVector(Vector2 vector, float radians)
    {
        float cosTheta = MathF.Cos(radians);
        float sinTheta = MathF.Sin(radians);

        float newX = vector.X * cosTheta - vector.Y * sinTheta;
        float newY = vector.X * sinTheta + vector.Y * cosTheta;

        return new Vector2(newX, newY);
    }

    public static void LogMessage(string message, LogType logType)
    {
        if (logType < EquipmentEvolved.LOG_LEVEL) return;

        ILog log = EquipmentEvolved.Instance.Logger;
        
        message = $"[{nameof(EquipmentEvolved)}] {message}";
        
        switch (logType)
        {
            case LogType.Debug:
                log.Debug(message);
                break;
            case LogType.Log:
                log.Info(message);
                break;
            case LogType.Warning:
                log.Warn(message);
                break;
            case LogType.Error:
                log.Error(message);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
        }
    }

    public static Rectangle GetCombatTextRect(NPC target)
    {
        return new Rectangle((int)target.position.X, (int)target.position.Y, 20, 20);
    }
    
    public static Rectangle GetCombatTextRect(Player target)
    {
        return new Rectangle((int)target.position.X, (int)target.position.Y, 20, 20);
    }
    
    public static void AnnounceNegated(Player player)
    {
        // 1. Show locally immediately
        CombatText.NewText(player.getRect(), Color.Green, "Negated!");

        // 2. Send packet to others
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            ModPacket packet = ModContent.GetInstance<EquipmentEvolved>().GetPacket();
            packet.Write((byte)MessageType.NegatedText);
            packet.Write((byte)player.whoAmI);
            packet.Send();
        }
    }
}