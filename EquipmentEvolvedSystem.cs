using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved;

public class EquipmentEvolvedSystem : ModSystem
{
    public static ModKeybind ArmorActivationKeybind;
    
    public override void Load()
    {
        LoadShaders();
    }

    public override void PostSetupContent()
    {
        ArmorActivationKeybind =
            KeybindLoader.RegisterKeybind(EquipmentEvolved.Instance, nameof(ArmorActivationKeybind), Keys.R);
    }

    private void LoadShaders()
    {
        if (Main.netMode == NetmodeID.Server) return;
        Asset<Effect> screenAsset = ModContent.Request<Effect>(
            "EquipmentEvolved/Assets/Effects/Phantom/SoulburnEffect", 
            AssetRequestMode.ImmediateLoad
        );
        Filters.Scene["EquipmentEvolved:SoulBurn"] = new Filter(
            new ScreenShaderData(screenAsset, "SoulBurnEffectPass"), 
            EffectPriority.VeryHigh
        );
    }
}