using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.ModPrefixes.Summoner.Whips.Sacrificial;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Thaumic;

public class ThaumicBuffManager : ModSystem
{
    public static List<int> ValidPositiveBuffs = [];
    
    public static HashSet<int> BannedBuffIDs =
    [
        BuffID.OnFire3,
        BuffID.Midas,
        BuffID.Featherfall
    ];
    
    public static HashSet<string> BannedBuffNames = new(StringComparer.OrdinalIgnoreCase)
    {
        //"CalamityMod/BrimstoneFlames",
    };

    public override void PostSetupContent()
    {
        ValidPositiveBuffs.Clear();

        BannedBuffIDs.Add(ModContent.BuffType<SacrificialBuff>());
        
        for (int i = 1; i < BuffLoader.BuffCount; i++)
        {
            if (Main.debuff[i] || Main.buffNoTimeDisplay[i] || Main.lightPet[i] || Main.vanityPet[i] || Main.persistentBuff[i]) continue;
            
            if (BannedBuffIDs.Contains(i)) continue;
            
            bool isBannedByName = false;
            
            ModBuff modBuff = BuffLoader.GetBuff(i);
            if (modBuff != null)
            {
                string fullInternalName = $"{modBuff.Mod.Name}/{modBuff.Name}";
                if (BannedBuffNames.Contains(fullInternalName) || BannedBuffNames.Contains(modBuff.Name)) isBannedByName = true;
            }
            
            string displayName = Lang.GetBuffName(i);
            if (!string.IsNullOrEmpty(displayName) && BannedBuffNames.Contains(displayName)) isBannedByName = true;

            if (isBannedByName) continue;
            
            ValidPositiveBuffs.Add(i);
        }
    }
}