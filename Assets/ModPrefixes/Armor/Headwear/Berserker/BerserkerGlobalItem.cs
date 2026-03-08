using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Headwear.Berserker;

public class BerserkerGlobalItem : GlobalItem
{
    public override void UpdateEquip(Item item, Player player)
    {
        if (!item.HasPrefix(ModContent.PrefixType<PrefixBerserker>())) return;

        float missingHealthPercent = 1f - (float)player.statLife / player.statLifeMax2;

        if (missingHealthPercent < 0f) missingHealthPercent = 0f;

        if (missingHealthPercent > 1f) missingHealthPercent = 1f;

        float damageBonus = missingHealthPercent * PrefixBalance.BERSERKER_MAX_DAMAGE_BONUS;

        player.GetModPlayer<StatPlayer>().DamageMul += damageBonus;
    }
}