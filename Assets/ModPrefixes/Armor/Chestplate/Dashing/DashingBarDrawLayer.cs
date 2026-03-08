using EquipmentEvolved.Assets.Balance;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Chestplate.Dashing;

public class DashingBarDrawLayer : PlayerDrawLayer
{
    public override Position GetDefaultPosition()
    {
        return new AfterParent(PlayerDrawLayers.FrontAccFront);
    }

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
        DashingModPlayer dashPlayer = drawInfo.drawPlayer.GetModPlayer<DashingModPlayer>();
        return drawInfo.shadow == 0f && !drawInfo.drawPlayer.dead && dashPlayer.IsDashingEquipped && dashPlayer.FadeAlpha > 0f;
    }

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;
        DashingModPlayer dashPlayer = player.GetModPlayer<DashingModPlayer>();

        Texture2D pixel = TextureAssets.MagicPixel.Value;
        Vector2 drawPos = player.Bottom - Main.screenPosition + new Vector2(0, 12);

        int maxCharges = PrefixBalance.DASHING_MAX_CHARGES;
        int segmentWidth = 12;
        int barHeight = 4;
        int spacing = 2;
        int totalWidth = maxCharges * segmentWidth + (maxCharges - 1) * spacing + 4;
        int totalHeight = barHeight + 4;

        drawPos.X -= totalWidth / 2f;
        drawPos.X = (int)drawPos.X;
        drawPos.Y = (int)drawPos.Y;

        float alpha = dashPlayer.FadeAlpha;

        Color frameColor = Color.Gray * alpha;
        Color emptyColor = new Color(0, 50, 50) * 0.5f * alpha;
        Color fillingColor = new Color(0, 150, 150) * 0.8f * alpha;
        Color fullColor = Color.Cyan * alpha;

        // 1. Draw Gray Frame
        Rectangle frameRect = new((int)drawPos.X, (int)drawPos.Y, totalWidth, totalHeight);
        drawInfo.DrawDataCache.Add(new DrawData(pixel, frameRect, frameColor));

        // 2. Draw Empty Background inside frame
        Rectangle bgRect = new((int)drawPos.X + 2, (int)drawPos.Y + 2, totalWidth - 4, barHeight);
        drawInfo.DrawDataCache.Add(new DrawData(pixel, bgRect, emptyColor));

        int currentX = (int)drawPos.X + 2;

        for (int i = 0; i < maxCharges; i++)
        {
            if (i < dashPlayer.Charges)
            {
                // Fully charged segment
                Rectangle segRect = new(currentX, (int)drawPos.Y + 2, segmentWidth, barHeight);
                drawInfo.DrawDataCache.Add(new DrawData(pixel, segRect, fullColor));
            }
            else if (i == dashPlayer.Charges)
            {
                // Currently filling segment
                float fillPercent = (float)dashPlayer.RechargeTimer / PrefixBalance.DASHING_RECHARGE_TICKS;
                int fillWidth = (int)(segmentWidth * fillPercent);
                if (fillWidth > 0)
                {
                    Rectangle segRect = new(currentX, (int)drawPos.Y + 2, fillWidth, barHeight);
                    drawInfo.DrawDataCache.Add(new DrawData(pixel, segRect, fillingColor));
                }
            }

            // Draw vertical separator line if it's not the last segment
            if (i < maxCharges - 1)
            {
                Rectangle sepRect = new(currentX + segmentWidth, (int)drawPos.Y + 2, spacing, barHeight);
                drawInfo.DrawDataCache.Add(new DrawData(pixel, sepRect, frameColor));
            }

            currentX += segmentWidth + spacing;
        }
    }
}