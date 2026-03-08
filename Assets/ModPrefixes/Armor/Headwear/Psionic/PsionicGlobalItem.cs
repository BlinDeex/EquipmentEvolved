using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Headwear.Psionic;

public class PsionicGlobalItem : GlobalItem
{
    public override void UpdateEquip(Item item, Player player)
    {
        if (!item.HasPrefix(ModContent.PrefixType<PrefixPsionic>())) return;

        player.treasureMagnet = true;
        player.manaMagnet = true;
        player.lifeMagnet = true;
    }
}