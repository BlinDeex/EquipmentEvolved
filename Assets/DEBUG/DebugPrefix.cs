using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.DEBUG;

public class DebugPrefix : ModPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult,
        ref float shootSpeedMult, ref float manaMult, ref int critBonus)
    {
        damageMult *= 100;
    }
}