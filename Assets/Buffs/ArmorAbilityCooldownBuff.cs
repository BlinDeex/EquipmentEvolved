using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Buffs;

public class ArmorAbilityCooldownBuff : ModBuff
{
    public override LocalizedText DisplayName => Mod.GetLocalization($"Buffs.{nameof(ArmorAbilityCooldownBuff)}");

    public override LocalizedText Description => Mod.GetLocalization($"Buffs.{nameof(ArmorAbilityCooldownBuff)}Desc");
    
    public override string Texture => $"{Mod.Name}/Assets/Textures/Buffs/ArmorAbilityCooldownBuff";

    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true; 
        Main.buffNoSave[Type] = true;
    }
}