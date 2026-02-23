using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.InstancedGlobalItems;

public class WeaponDivideZeroFix : GlobalItem
{
    public override bool CanUseItem(Item item, Player player)
    {
        //if (item.useTime <= 0) item.useTime = 1; TODO: looks like this is not needed anymore
        //if (item.useAnimation <= 0) item.useAnimation = 1;
        return true;
    }
}