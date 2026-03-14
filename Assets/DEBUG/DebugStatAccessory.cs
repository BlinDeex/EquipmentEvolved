using System.Collections.Generic;
using System.Linq;
using EquipmentEvolved.Assets.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace EquipmentEvolved.Assets.DEBUG; // Ensure this matches your folder structure!

// ==========================================
// 1. ORIGINAL DEBUG ACCESSORY (Filtered List)
// ==========================================
public class DebugStatAccessory : ModItem
{
    public override string Texture => "Terraria/Images/Item_" + ItemID.Radar;

    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 28;
        Item.accessory = true;
        Item.rare = ItemRarityID.Red;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<DebugStatPlayer>().ShowStats = true;
    }
}

// ==========================================
// 2. NEW DEBUG ACCESSORY (Full Gray/Green List)
// ==========================================
public class DebugAllStatAccessory : ModItem
{
    // Using a different texture so you can tell them apart!
    public override string Texture => "Terraria/Images/Item_" + ItemID.GPS;

    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 28;
        Item.accessory = true;
        Item.rare = ItemRarityID.Red;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<DebugStatPlayer>().ShowAllStats = true;
    }
}

// ==========================================
// 3. PLAYER DATA
// ==========================================
public class DebugStatPlayer : ModPlayer
{
    public bool ShowStats;
    public bool ShowAllStats; // NEW: Track the new accessory

    public override void ResetEffects()
    {
        ShowStats = false;
        ShowAllStats = false;
    }
}

// ==========================================
// 4. UI DRAWING
// ==========================================
public class DebugStatUI : ModSystem
{
    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (mouseTextIndex != -1)
        {
            layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer("EquipmentEvolved: Debug Stats", delegate
            {
                DrawDebugStats();
                DrawAllDebugStats(); // NEW: Call the full list drawer
                return true;
            }));
        }
    }

    private static void DrawDebugStats()
    {
        Player player = Main.LocalPlayer;

        if (!player.active || player.dead || !player.GetModPlayer<DebugStatPlayer>().ShowStats) return;

        StatPlayer statPlayer = player.GetModPlayer<StatPlayer>();
        string text = "--- ACTIVE STATS ---\n";

        foreach (EquipmentStat stat in ModContent.GetContent<EquipmentStat>())
        {
            float totalValue = statPlayer.GetTotalStat(stat);
            if (totalValue != 0f)
            {
                text += $"{stat.Name}: {totalValue:0.##}\n";
            }
        }

        float textScale = 0.67f;
        Vector2 textSize = FontAssets.MouseText.Value.MeasureString(text) * textScale;
        Vector2 drawPos = new(player.Right.X - Main.screenPosition.X + 32, player.Center.Y - Main.screenPosition.Y - textSize.Y / 2f);

        Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, text, drawPos.X, drawPos.Y, Color.LimeGreen, Color.Black, Vector2.Zero, textScale);
    }

    private static void DrawAllDebugStats()
    {
        Player player = Main.LocalPlayer;

        if (!player.active || player.dead || !player.GetModPlayer<DebugStatPlayer>().ShowAllStats) return;

        StatPlayer statPlayer = player.GetModPlayer<StatPlayer>();
        ReLogic.Graphics.DynamicSpriteFont font = FontAssets.MouseText.Value;
        float textScale = 0.67f;

        var allStats = ModContent.GetContent<EquipmentStat>().ToList();
        
        // Calculate the height of a single line, and the total height to keep it vertically centered
        float lineHeight = font.MeasureString("A").Y * textScale;
        float totalHeight = (allStats.Count + 1) * lineHeight; // +1 for the header

        // Starting position (Centered vertically based on the total list height)
        Vector2 currentPos = new(player.Right.X - Main.screenPosition.X + 32, player.Center.Y - Main.screenPosition.Y - (totalHeight / 2f));

        // Draw Header
        Utils.DrawBorderStringFourWay(Main.spriteBatch, font, "--- ALL STATS DEBUG ---", currentPos.X, currentPos.Y, Color.White, Color.Black, Vector2.Zero, textScale);
        currentPos.Y += lineHeight;

        // Draw each stat line-by-line so we can apply different colors!
        foreach (EquipmentStat stat in allStats)
        {
            float totalValue = statPlayer.GetTotalStat(stat);
            
            // Green if active, Gray if inactive (0f)
            Color textColor = totalValue != 0f ? Color.LimeGreen : Color.Gray;
            string text = $"{stat.Name}: {totalValue:0.##}";

            Utils.DrawBorderStringFourWay(Main.spriteBatch, font, text, currentPos.X, currentPos.Y, textColor, Color.Black, Vector2.Zero, textScale);
            
            // Move down for the next line
            currentPos.Y += lineHeight;
        }
    }
}