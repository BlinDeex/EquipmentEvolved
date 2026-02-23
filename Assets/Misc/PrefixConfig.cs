using Terraria.ModLoader.Config;

namespace EquipmentEvolved.Assets.Misc;

public class PrefixConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    public bool AllowVanillaPrefixes { get; set; }
    public bool ExperimentalReforges { get; set; }
}