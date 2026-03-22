using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.Phantom;

public class SoulBurnBuff : ModBuff
{
    public override LocalizedText DisplayName => Mod.GetLocalization($"Buffs.{nameof(SoulBurnBuff)}");

    public override LocalizedText Description => Mod.GetLocalization($"Buffs.{nameof(SoulBurnBuff)}Desc");

    public override string Texture => $"{Mod.Name}/Assets/Textures/Buffs/SacrificialIcon";

    public override bool ReApply(Player player, int time, int buffIndex)
    {
        int index = player.FindBuffIndex(Type);
        int totalTime = player.buffTime[index] + time;
        if (totalTime > 1200) totalTime = 1200;
        player.buffTime[index] = totalTime;
        return true;
    }

    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        Main.buffNoSave[Type] = true;
    }
}