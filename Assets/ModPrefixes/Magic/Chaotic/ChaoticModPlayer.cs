using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Magic.Chaotic;

public class ChaoticModPlayer : ModPlayer
{
    public override void UpdateEquips()
    {
        bool hasChaotic = false;
        bool isHoldingChaotic = false;

        if (Player.HeldItem != null && !Player.HeldItem.IsAir && Player.HeldItem.HasPrefix(ModContent.PrefixType<PrefixChaotic>()))
        {
            hasChaotic = true;
            isHoldingChaotic = true;
        }
        else
        {
            for (int i = 0; i < 58; i++) // 0-49 is inventory, 50-53 coins, 54-57 ammo
            {
                Item item = Player.inventory[i];
                if (item != null && !item.IsAir && item.HasPrefix(ModContent.PrefixType<PrefixChaotic>()))
                {
                    hasChaotic = true;
                    break;
                }
            }
        }

        if (hasChaotic) ChaoticRollPool.Tick(Player, isHoldingChaotic);
    }
}