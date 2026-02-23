using Terraria;
using Terraria.DataStructures;

namespace EquipmentEvolved.Assets;

public class MiscStuff
{
    public static PlayerDeathReason GetManaSurgeDeath(string playerName) =>
        PlayerDeathReason.ByCustomReason($"{playerName} was not capable enough to withstand mana surge");

    public static PlayerDeathReason CHALLENGER_RED_ORB_DEATH =>
        PlayerDeathReason.ByCustomReason($"{Main.LocalPlayer.name} hit the wrong orb one too many times");

    public static PlayerDeathReason CHAOTIC_WEAPON_DEATH =>
        PlayerDeathReason.ByCustomReason($"{Main.LocalPlayer.name} weapon was too chaotic");
}