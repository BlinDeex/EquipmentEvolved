using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Summoner.MinionWeapons.Proxy;

public class PrefixProxy : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;

    public static LocalizedText Desc { get; private set; }

    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, "Proxy", "DisplayName");

    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.MinionWeapon;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }

    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this, "Proxy", nameof(Desc));
    }
    
    public override bool CanRoll(Item item)
    {
        if (item.type is ItemID.StormTigerStaff or ItemID.StardustDragonStaff or ItemID.AbigailsFlower) return false;

        return true;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        TooltipLine newLine = new(Mod, "ProxyDesc", Desc.Format(MathF.Round(PrefixBalance.PROXY_MAX_DURATION_TICKS / 60f, 1), MathF.Round(PrefixBalance.PROXY_DAMAGE_EFFICIENCY * 100)))
        {
            IsModifier = true
        };

        yield return newLine;
    }
}