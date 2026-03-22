using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Summoner.MinionWeapons.Proxy;

public class PrefixProxy : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;
    public override float ReforgeMultiplier => PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.MinionWeapon;

    public override bool CanRoll(Item item)
    {
        if (item.type is ItemID.StormTigerStaff or ItemID.StardustDragonStaff or ItemID.AbigailsFlower) return false;
        return true;
    }

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "ProxyDesc", Description.Format(MathF.Round(PrefixBalance.PROXY_MAX_DURATION_TICKS, 1)))
        {
            IsModifier = true
        };
    }
}