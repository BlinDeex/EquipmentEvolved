using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Challenger;

public class ChallengerGlobalProjectile : GlobalProjectile
{
    public bool IsChallenger;
    public override bool InstancePerEntity => true;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source is IEntitySource_WithStatsFromItem itemSource && itemSource.Item.HasPrefix(ModContent.PrefixType<PrefixChallenger>())) IsChallenger = true;
    }
}