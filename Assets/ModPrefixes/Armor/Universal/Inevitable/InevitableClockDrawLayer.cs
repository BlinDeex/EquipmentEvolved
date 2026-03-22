using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.Inevitable;

public class InevitableClockDrawLayer : PlayerDrawLayer
{
    public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.JimsCloak);

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;
        InevitableArmorPlayer inevitablePlayer = player.GetModPlayer<InevitableArmorPlayer>();

        if (inevitablePlayer.InevitableTimer <= 0) return;

        Texture2D pixel = TextureAssets.MagicPixel.Value;
        
        Vector2 drawCenter = player.MountedCenter - Main.screenPosition + new Vector2(0, -60f);
        float radius = 70f;
        
        float globalAlpha = 0.7f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 4f) * 0.2f;
        Color baseGlowColor = new Color(0, 255, 255, 0) * globalAlpha;
        Color solidColor = Color.White * globalAlpha;
        
        for (int i = 0; i < 12; i++)
        {
            float angle = MathHelper.ToRadians((i * 30) - 90);
            Vector2 dotPos = drawCenter + angle.ToRotationVector2() * radius;
            
            DrawData glowData = new DrawData(pixel, dotPos, new Rectangle(0, 0, 1, 1), baseGlowColor, 0f, new Vector2(0.5f, 0.5f), 6f, SpriteEffects.None, 0);
            drawInfo.DrawDataCache.Add(glowData);
            
            DrawData solidData = new DrawData(pixel, dotPos, new Rectangle(0, 0, 1, 1), solidColor, 0f, new Vector2(0.5f, 0.5f), 2f, SpriteEffects.None, 0);
            drawInfo.DrawDataCache.Add(solidData);
        }
        
        int currentSecond = (int)Math.Ceiling(inevitablePlayer.InevitableTimer / 60f);
        
        int hourProgress = 12 - currentSecond;
        float handAngle = MathHelper.ToRadians((hourProgress * 30) - 90);
        
        float handLength = radius * 0.85f;
        float handThickness = 3f;
        
        DrawData handGlow = new DrawData(pixel, drawCenter, new Rectangle(0, 0, 1, 1), baseGlowColor, handAngle, new Vector2(0f, 0.5f), new Vector2(handLength, handThickness * 2.5f), SpriteEffects.None, 0);
        drawInfo.DrawDataCache.Add(handGlow);
        
        DrawData handSolid = new DrawData(pixel, drawCenter, new Rectangle(0, 0, 1, 1), solidColor, handAngle, new Vector2(0f, 0.5f), new Vector2(handLength, handThickness), SpriteEffects.None, 0);
        drawInfo.DrawDataCache.Add(handSolid);
    }
}