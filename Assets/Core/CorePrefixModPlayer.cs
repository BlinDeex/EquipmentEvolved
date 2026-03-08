using System.Linq;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Misc;
using Terraria;
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

    public override void PostUpdateMiscEffects()
    {
        HandleWaterBreathing();
    }

    private bool HasReforgedItemInInventory()
    {
        return Player.inventory.Any(item => item.prefix != 0);
    }

    private void HandleWaterBreathing()
    {
        if (Main.myPlayer != Player.whoAmI) return;

        if (!Player.TryGetModPlayer(out MiscArmorPlayer miscArmorPlayer)) return;

        if (!miscArmorPlayer.WaterBreathing) return;

        if (Player.breath >= Player.breathMax) return;

        if (Player.breathCD != Player.breathCDMax - 1) return;

        if (Main.rand.NextFloat() <= 0.67f) Player.breath++;
    }
}