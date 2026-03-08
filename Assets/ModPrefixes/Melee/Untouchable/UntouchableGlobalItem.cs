using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.Untouchable;

public class UntouchableGlobalItem : GlobalItem
{
    public override bool InstancePerEntity => true;

    public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
    {
        if (!item.HasPrefix(ModContent.PrefixType<PrefixUntouchable>())) return;

        UntouchableModPlayer prefixPlayer = player.GetModPlayer<UntouchableModPlayer>();
        damage *= prefixPlayer.UntouchableDamageIncrease + 1;
    }
}