using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.Phantom;

public class PhantomModSystem : ModSystem
{
    public override void Load()
    {
        if (Main.netMode == NetmodeID.Server) return;

        Asset<Effect> screenAsset = ModContent.Request<Effect>("EquipmentEvolved/Assets/Effects/Phantom/SoulburnEffect", AssetRequestMode.ImmediateLoad);

        Filters.Scene["EquipmentEvolved:SoulBurn"] = new Filter(new ScreenShaderData(screenAsset, "SoulBurnEffectPass"), EffectPriority.VeryHigh);
    }
}