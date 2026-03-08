using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.ArcaneInfused;

public class ArcaneInfusedGlobalItem : GlobalItem
{
    public override bool InstancePerEntity => true;

    public override bool CanUseItem(Item item, Player player)
    {
        if (!item.HasPrefix(ModContent.PrefixType<PrefixArcaneInfused>())) return base.CanUseItem(item, player);

        bool usedMana = player.CheckMana(10, true);
        player.manaRegenDelay = 300;
        return usedMana;
    }

    public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
    {
        if (!item.HasPrefix(ModContent.PrefixType<PrefixArcaneInfused>()) || !player.HasBuff(BuffID.ManaSickness)) return;

        int index = player.FindBuffIndex(BuffID.ManaSickness);
        int timeLeft = player.buffTime[index];
        float targetReduction = timeLeft * 0.1f;
        damage.Flat = 1f - targetReduction;
    }
}