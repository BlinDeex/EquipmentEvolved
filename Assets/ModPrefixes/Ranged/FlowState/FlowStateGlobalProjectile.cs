using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.FlowState;

public class FlowStateGlobalProjectile : GlobalProjectile
{
    public bool IsFlowState;
    public override bool InstancePerEntity => true;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source is IEntitySource_WithStatsFromItem itemSource && itemSource.Item.HasPrefix(ModContent.PrefixType<PrefixFlowState>())) IsFlowState = true;
    }

    public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (!IsFlowState) return;

        FlowStateNPC flowNPC = target.GetGlobalNPC<FlowStateNPC>();

        flowNPC.FlowStacks++;
        flowNPC.FlowTimer = PrefixBalance.FLOW_STATE_DURATION_TICKS;
        flowNPC.BaseWeaponDamage = projectile.damage;
        flowNPC.DecayTimer = 0;
    }
}