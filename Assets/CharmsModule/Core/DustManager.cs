using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.CharmsModule.Core;

public static class DustManager
{
    private static Asset<Texture2D> _noiseTexture;

    public static void DrawDustShaderStatic(SpriteBatch spriteBatch, Vector2 position, Color dustColor, float scale, Matrix transformMatrix, float seed)
    {
        Effect shader = ModContent.Request<Effect>("EquipmentEvolved/Assets/Effects/Charms/DustShader", AssetRequestMode.ImmediateLoad).Value;
        if (shader == null) return;

        shader.Parameters["uColor"]?.SetValue(dustColor.ToVector3());
        shader.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly); 
        shader.Parameters["uPixelation"]?.SetValue(16f * scale); 
        
        // FIX: Use the perfectly stable seed passed from the item
        shader.Parameters["uSeed"]?.SetValue(seed);

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, transformMatrix);

        shader.CurrentTechnique.Passes[0].Apply();

        Texture2D blankTex = TextureAssets.MagicPixel.Value;
        
        // WE CHANGED THIS LINE: 24f -> 36f
        // This physically expands the drawing canvas, scaling the entire shader up flawlessly!
        Vector2 canvasSize = new Vector2(36f, 36f) * scale; 
        
        Vector2 origin = blankTex.Size() / 2f;
        Vector2 scaleVec = canvasSize / blankTex.Size();

        spriteBatch.Draw(blankTex, position, null, Color.White, 0f, origin, scaleVec, SpriteEffects.None, 0f);

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, transformMatrix);
    }
}