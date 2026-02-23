using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Buffs;

public class VoidSealBuff : ModBuff
{
    public override LocalizedText DisplayName => Mod.GetLocalization($"Buffs.{nameof(VoidSealBuff)}");

    public override LocalizedText Description => Mod.GetLocalization($"Buffs.{nameof(VoidSealBuff)}Desc");

    public override string Texture => $"{Mod.Name}/Assets/Textures/Buffs/SacrificialIcon";

    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true; 
        Main.buffNoSave[Type] = true;
    }
}