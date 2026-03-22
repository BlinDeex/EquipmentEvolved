using System.Linq;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Core;

public class CorePrefixModPlayer : ModPlayer
{
    private int tickTimer;
    public bool HasReforgedItemInv { get; private set; }

    public override void PostUpdateEquips()
    {
        tickTimer++;
        if (tickTimer % 60 == 0) HasReforgedItemInv = HasReforgedItemInInventory();
    }

    private bool HasReforgedItemInInventory()
    {
        return Player.inventory.Any(item => item.prefix != 0);
    }
}