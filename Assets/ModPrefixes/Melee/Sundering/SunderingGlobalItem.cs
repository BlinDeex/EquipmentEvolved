using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.Sundering;

public class SunderingGlobalItem : GlobalItem
{
    public override bool InstancePerEntity => true;

    public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (!item.HasPrefix(ModContent.PrefixType<PrefixSundering>())) return;

        SunderingGlobalNPC globalNPC = target.GetGlobalNPC<SunderingGlobalNPC>();
        float shredAmount = target.defDefense * PrefixBalance.SUNDERING_DEFENSE_REDUCTION;

        globalNPC.SunderedDefense += shredAmount;
        target.netUpdate = true;
    }
}