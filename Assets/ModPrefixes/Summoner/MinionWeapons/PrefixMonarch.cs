using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Globals.Projectiles;
using EquipmentEvolved.Assets.InstancedGlobalItems;
using EquipmentEvolved.Assets.Misc;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Summoner.MinionWeapons;

public class PrefixMonarch : ModPrefix, ISpecializedPrefix
{
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.MinionWeapon;
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER * 1.25f; 
    }

    public static LocalizedText Desc { get; private set; }
    
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this, "Monarch", "DisplayName");

    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this, "Monarch", nameof(Desc));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        // 1. Standard Description
        yield return new TooltipLine(Mod, "Desc",
            Desc.Format(
                (PrefixBalance.MONARCH_EFFICIENCY_BONUS_PER_STACK * 100).ToString("0"), 
                (PrefixBalance.MONARCH_SIZE_PER_STACK * 100).ToString("0")
            ))
        {
            OverrideColor = Color.Gold
        };
        
        yield return new TooltipLine(Mod, "Mechanic", 
            "Summoning again feeds the Monarch, increasing Size, Damage, and Slot usage.")
        {
            OverrideColor = Color.Lerp(Color.Gold, Color.White, 0.5f)
        };

        // 2. Dynamic Status Search
        // Find if the player has an active Monarch corresponding to THIS specific weapon
        Projectile activeMonarch = null;
        int projType = item.shoot; // The projectile this staff shoots

        for (int i = 0; i < Main.maxProjectiles; i++)
        {
            Projectile p = Main.projectile[i];
            // Must match Type, Owner, and be a Monarch
            if (p.active && p.owner == Main.myPlayer && p.type == projType)
            {
                if (p.GetGlobalProjectile<InstancedProjectilePrefix>().Monarch)
                {
                    activeMonarch = p;
                    break;
                }
            }
        }

        // 3. Display Status
        if (activeMonarch != null)
        {
            InstancedProjectilePrefix prefix = activeMonarch.GetGlobalProjectile<InstancedProjectilePrefix>();
            (float damageMult, float scale, float slots) stats = SummonerProjectile.GetMonarchStats(prefix.MonarchStacks);

            // Format: "Current: 450% Dmg | 3.5x Size | 5 Slots"
            string status = $"[Active] Dmg: {stats.damageMult * 100:0}% | Size: {stats.scale:0.0}x | Slots: {stats.slots:0.#}";
            
            yield return new TooltipLine(Mod, "Status", status)
            {
                OverrideColor = Color.LimeGreen // bright green for active status
            };
        }
        else
        {
            yield return new TooltipLine(Mod, "Status", "[Inactive]")
            {
                OverrideColor = Color.Gray
            };
        }
    }
}