using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Terraria.ModLoader.Config;

namespace EquipmentEvolved.Assets.Misc;

[BackgroundColor(20, 20, 25, 155)]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class PrefixConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    [Header("CoreSystems")]
    [BackgroundColor(80, 40, 40)]
    [DefaultValue(true)]
    public bool EnableModdedReforges { get; set; }

    [BackgroundColor(80, 40, 40)]
    [DefaultValue(true)]
    public bool EnableCharms { get; set; }

    [Header("EquipmentTypes")]
    [BackgroundColor(40, 80, 40)]
    [DefaultValue(true)]
    public bool EnableArmorReforging { get; set; }

    [BackgroundColor(40, 80, 40)]
    [DefaultValue(true)]
    public bool EnableToolReforging { get; set; }
    
    [Header("Advanced")]
    [BackgroundColor(40, 40, 80)]
    [DefaultValue(true)]
    public bool EnableExperimentalReforges { get; set; }

    [BackgroundColor(40, 40, 80)]
    [DefaultValue(false)]
    public bool EnableVanillaReforges { get; set; }
}