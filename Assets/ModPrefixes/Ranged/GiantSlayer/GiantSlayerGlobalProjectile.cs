using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.GiantSlayer;

public class GiantSlayerGlobalProjectile : GlobalProjectile
{
    public bool IsGiantSlayer;
    public override bool InstancePerEntity => true;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source is IEntitySource_WithStatsFromItem itemSource && itemSource.Item.HasPrefix(ModContent.PrefixType<PrefixGiantSlayer>())) IsGiantSlayer = true;
    }

    public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
    {
        if (!IsGiantSlayer) return;

        WeaponUtils.FullHitCancellation(target, ref modifiers);
    }
}