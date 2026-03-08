using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace EquipmentEvolved.Assets.DEBUG;

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

public class DebugStatPlayer : ModPlayer
{
    public bool ShowStats;

    public override void ResetEffects()
    {
        ShowStats = false;
    }
}

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
                return true;
            }));
        }
    }

    private static void DrawDebugStats()
    {
        Player player = Main.LocalPlayer;

        if (!player.active || player.dead || !player.GetModPlayer<DebugStatPlayer>().ShowStats) return;

        StatPlayer statPlayer = player.GetModPlayer<StatPlayer>();

        string text = "--- STAT PLAYER DEBUG ---\n";

        PlayerStat[] statNames = (PlayerStat[])Enum.GetValues(typeof(PlayerStat));

        for (int i = 0; i < statNames.Length; i++)
        {
            text += $"{statNames[i]}: {statPlayer.ActiveStats[i]:0.##}\n";
        }

        text += $"DefenseMul: {statPlayer.DefenseMul:0.##}\n";
        text += $"AdditionalMinions: {statPlayer.AdditionalMinions:0.##}\n";

        float textScale = 0.67f;

        Vector2 textSize = FontAssets.MouseText.Value.MeasureString(text) * textScale;

        Vector2 drawPos = new(player.Right.X - Main.screenPosition.X + 32, player.Center.Y - Main.screenPosition.Y - textSize.Y / 2f);

        Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, text, drawPos.X, drawPos.Y, Color.LimeGreen, Color.Black, Vector2.Zero, textScale);
    }
}