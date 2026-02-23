using Terraria.ID;

namespace EquipmentEvolved.Assets.Utilities;

public static class AdaptableUtils
{
    public static readonly RocketAndProjID[] ROCKET_TO_PROJ_IDS =
    [
        new(ItemID.RocketI, ProjectileID.RocketI),
        new(ItemID.RocketII, ProjectileID.RocketII),
        new(ItemID.RocketIII, ProjectileID.RocketIII),
        new(ItemID.RocketIV, ProjectileID.RocketIV),
        new(ItemID.ClusterRocketI, ProjectileID.ClusterRocketI),
        new(ItemID.ClusterRocketII, ProjectileID.ClusterRocketII),
        new(ItemID.DryRocket, ProjectileID.DryRocket),
        new(ItemID.WetRocket, ProjectileID.WetRocket),
        new(ItemID.LavaRocket, ProjectileID.LavaRocket),
        new(ItemID.HoneyRocket, ProjectileID.HoneyRocket),
        new(ItemID.MiniNukeI, ProjectileID.MiniNukeRocketI),
        new(ItemID.MiniNukeII, ProjectileID.MiniNukeRocketII)
    ];

    public readonly struct RocketAndProjID(int rocketAmmoID, int rocketProjectileID)
    {
        public int RocketAmmoID => rocketAmmoID;
        public int RocketProjectileID => rocketProjectileID;
    }

    public static readonly int[] ROCKET_WEAPON_IDS =
    [
        ItemID.GrenadeLauncher,
        ItemID.RocketLauncher,
        ItemID.ProximityMineLauncher,
        ItemID.SnowmanCannon,
        ItemID.ElectrosphereLauncher,
        ItemID.FireworksLauncher,
        ItemID.Celeb2
    ];
}