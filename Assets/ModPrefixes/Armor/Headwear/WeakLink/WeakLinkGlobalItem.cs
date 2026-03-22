using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Headwear.WeakLink;

public class WeakLinkGlobalItem : GlobalItem
{
    public override void UpdateEquip(Item item, Player player)
    {
        if (!item.HasPrefix(ModContent.PrefixType<PrefixWeakLink>())) return;
        
        int defenseLoss = (int)(item.defense * (1f - PrefixBalance.WEAK_LINK_THIS_DEFENSE_MULT));
        player.statDefense -= defenseLoss;

        Item chest = player.armor[1];
        Item legs = player.armor[2];
        int bonusDefense = 0;

        if (chest != null && !chest.IsAir) bonusDefense += (int)(chest.defense * (PrefixBalance.WEAK_LINK_OTHER_DEFENSE_MULT - 1f));
        if (legs != null && !legs.IsAir) bonusDefense += (int)(legs.defense * (PrefixBalance.WEAK_LINK_OTHER_DEFENSE_MULT - 1f));

        player.statDefense += bonusDefense;
    }
}