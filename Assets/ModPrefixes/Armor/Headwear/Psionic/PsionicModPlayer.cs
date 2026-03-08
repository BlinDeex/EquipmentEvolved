using EquipmentEvolved.Assets.Balance;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Headwear.Psionic;

public class PsionicModPlayer : ModPlayer
{
    public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
    {
        if (Player.armor[0] == null || Player.armor[0].IsAir || Player.armor[0].prefix != ModContent.PrefixType<PrefixPsionic>()) return;

        if (item.type == ItemID.Heart || item.type == ItemID.CandyApple || item.type == ItemID.CandyCane) healValue = (int)(healValue * PrefixBalance.PSIONIC_HEAL_BONUS_MULT);
    }

    public override void GetHealMana(Item item, bool quickHeal, ref int healValue)
    {
        if (Player.armor[0] == null || Player.armor[0].IsAir || Player.armor[0].prefix != ModContent.PrefixType<PrefixPsionic>()) return;

        if (item.type == ItemID.Star || item.type == ItemID.SoulCake || item.type == ItemID.SugarPlum) healValue = (int)(healValue * PrefixBalance.PSIONIC_HEAL_BONUS_MULT);
    }
}