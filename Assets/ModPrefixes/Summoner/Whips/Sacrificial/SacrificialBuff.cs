using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Summoner.Whips.Sacrificial;

public class SacrificialBuff : ModBuff
{
    public override LocalizedText DisplayName => Mod.GetLocalization($"Buffs.{nameof(SacrificialBuff)}");

    public override LocalizedText Description => Mod.GetLocalization($"Buffs.{nameof(SacrificialBuff)}Desc");

    public override string Texture => $"{Mod.Name}/Assets/Textures/Buffs/SacrificialIcon";
}