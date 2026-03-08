using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Adaptable;

public class AdaptableGlobalItem : GlobalItem
{
    public override bool? CanChooseAmmo(Item weapon, Item ammo, Player player)
    {
        if (weapon.HasPrefix(ModContent.PrefixType<PrefixAdaptable>())) return true;

        return null;
    }
}