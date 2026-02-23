using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EquipmentEvolved.Assets.Utilities.Spritebatch;

// found on discord by prim lord
public struct SpriteBatchSnapshot(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState,
    RasterizerState rasterizerState, Effect effect, Matrix transformMatrix)
{
    public SpriteSortMode SortMode = sortMode;
    public BlendState BlendState = blendState;
    public SamplerState SamplerState = samplerState;
    public DepthStencilState DepthStencilState = depthStencilState;
    public RasterizerState RasterizerState = rasterizerState;
    public Effect Effect = effect;
    public Matrix TransformMatrix = transformMatrix;

    public void Begin(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(in this);
    }

    public static SpriteBatchSnapshot Capture(SpriteBatch spriteBatch)
    {
        SpriteSortMode sortMode = (SpriteSortMode)SpriteBatchSnapshotCache.SortModeField.GetValue(spriteBatch)!;

        BlendState blendState = (BlendState)SpriteBatchSnapshotCache.BlendStateField.GetValue(spriteBatch);

        SamplerState samplerState = (SamplerState)SpriteBatchSnapshotCache.SamplerStateField.GetValue(spriteBatch);

        DepthStencilState depthStencilState =
            (DepthStencilState)SpriteBatchSnapshotCache.DepthStencilStateField.GetValue(spriteBatch);

        RasterizerState rasterizerState =
            (RasterizerState)SpriteBatchSnapshotCache.RasterizerStateField.GetValue(spriteBatch);

        Effect effect = (Effect)SpriteBatchSnapshotCache.EffectField.GetValue(spriteBatch);

        Matrix transformMatrix = (Matrix)SpriteBatchSnapshotCache.TransformMatrixField.GetValue(spriteBatch)!;

        return new SpriteBatchSnapshot(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect,
            transformMatrix);
    }
}