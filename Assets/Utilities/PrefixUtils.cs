using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.ModPrefixes.Magic.Chaotic;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Utilities;

public static class PrefixUtils
{
    public static bool HasPrefix(this Item item, int targetPrefixId)
    {
        if (item.prefix == targetPrefixId) return true;
        
        if (item.prefix != ModContent.PrefixType<PrefixChaotic>()) return false;
        return ChaoticRollPool.CurrentFakedPrefixId == targetPrefixId;
    }
}