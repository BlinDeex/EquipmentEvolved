using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Summoner.MinionWeapons.Monarch;

public class PrefixMonarch : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;

    public static LocalizedText Desc { get; private set; }

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Monarch", "DisplayName");

    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.MinionWeapon;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER * 1.25f;
    }

    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this, "Monarch", nameof(Desc));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "Desc", Desc.Format((PrefixBalance.MONARCH_EFFICIENCY_BONUS_PER_STACK * 100).ToString("0"), (PrefixBalance.MONARCH_SIZE_PER_STACK * 100).ToString("0")))
        {
            OverrideColor = Color.Gold
        };

        yield return new TooltipLine(Mod, "Mechanic", "Summoning again feeds the Monarch, increasing Size, Damage, and Slot usage.")
        {
            OverrideColor = Color.Lerp(Color.Gold, Color.White, 0.5f)
        };
        Projectile activeMonarch = null;
        int projType = item.shoot;

        for (int i = 0; i < Main.maxProjectiles; i++)
        {
            Projectile p = Main.projectile[i];
            if (!p.active || p.owner != Main.myPlayer || p.type != projType) continue;
            if (!p.GetGlobalProjectile<MonarchGlobalProjectile>().Monarch) continue;
            activeMonarch = p;
            break;
        }
        
        if (activeMonarch != null)
        {
            MonarchGlobalProjectile monarchGP = activeMonarch.GetGlobalProjectile<MonarchGlobalProjectile>();
            (float damageMult, float scale, float slots) stats = MonarchGlobalProjectile.GetMonarchStats(monarchGP.MonarchStacks);
            
            string status = $"[Active] Dmg: {stats.damageMult * 100:0}% | Size: {stats.scale:0.0}x | Slots: {stats.slots:0.#}";

            yield return new TooltipLine(Mod, "Status", status)
            {
                OverrideColor = Color.LimeGreen
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