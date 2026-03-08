using EquipmentEvolved.Assets.Misc;
using Terraria.DataStructures;

namespace EquipmentEvolved.Assets.Utilities;

public class MiscStuff
{
    public static PlayerDeathReason GetManaSurgeDeath(string playerName)
    {
        return PlayerDeathReason.ByCustomReason(LocalizationManager.GetDeathReasonText("ManaSurge").ToNetworkText(playerName));
    }

    public static PlayerDeathReason GetChallengerRedOrbDeath(string playerName)
    {
        return PlayerDeathReason.ByCustomReason(LocalizationManager.GetDeathReasonText("ChallengerRedOrb").ToNetworkText(playerName));
    }

    public static PlayerDeathReason GetChaoticWeaponDeath(string playerName)
    {
        return PlayerDeathReason.ByCustomReason(LocalizationManager.GetDeathReasonText("ChaoticWeapon").ToNetworkText(playerName));
    }
}