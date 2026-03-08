using Microsoft.Xna.Framework.Input;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Core;

public class ArmorKeybindSystem : ModSystem
{
    public static ModKeybind ArmorActivationKeybind { get; private set; }

    public override void Load()
    {
        ArmorActivationKeybind = KeybindLoader.RegisterKeybind(Mod, "ArmorActivationKeybind", Keys.R);
    }

    public override void Unload()
    {
        ArmorActivationKeybind = null;
    }
}