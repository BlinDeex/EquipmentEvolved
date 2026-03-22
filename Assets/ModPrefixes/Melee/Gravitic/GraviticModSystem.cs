using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.Gravitic;

public class GraviticModSystem : ModSystem
{
    public override void Load()
    {
        if (Main.netMode == NetmodeID.Server) return;

        Asset<Effect> screenAsset = ModContent.Request<Effect>("EquipmentEvolved/Assets/Effects/Gravitic/GraviticWave", AssetRequestMode.ImmediateLoad);

        Filters.Scene["EquipmentEvolved:GraviticWave"] = new Filter(new ScreenShaderData(screenAsset, "CollapsingWavePass"), EffectPriority.High);
    }
}