using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Ascendant;

public class AscendantGlobalProjectile : GlobalProjectile
{
    public Item SourceWeapon;
    public override bool InstancePerEntity => true;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source is not IEntitySource_WithStatsFromItem itemSource) return;
        if (itemSource.Item.HasPrefix(ModContent.PrefixType<PrefixAscendant>())) SourceWeapon = itemSource.Item;
    }

    public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (SourceWeapon == null) return;
        if (SourceWeapon.TryGetGlobalItem(out AscendantGlobalItem ascendantItem)) ascendantItem.DamageDone += damageDone;
    }
}