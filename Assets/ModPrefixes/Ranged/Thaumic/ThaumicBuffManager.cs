using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Thaumic;

public class ThaumicBuffManager : ModSystem
{
    public static List<int> ValidPositiveBuffs = [];

    // 1. Explicit Banlist for Vanilla Buffs (or modded if you have the ID)
    public static HashSet<int> BannedBuffIDs =
    [
        BuffID.OnFire3,
        BuffID.Midas,
        BuffID.Featherfall
    ];

    // 2. Explicit Banlist by String Name (Great for cross-mod support without dependencies)
    // Uses OrdinalIgnoreCase so "hellfire" matches "Hellfire"
    public static HashSet<string> BannedBuffNames = new(System.StringComparer.OrdinalIgnoreCase)
    {
        //"CalamityMod/BrimstoneFlames",
    };

    public override void PostSetupContent()
    {
        ValidPositiveBuffs.Clear();
        
        BannedBuffIDs.Add(ModContent.BuffType<Buffs.SacrificialBuff>());
        // Loop through all vanilla and modded buffs
        for (int i = 1; i < BuffLoader.BuffCount; i++)
        {
            // --- FILTER 1: Vanilla Flags ---
            // Filter out debuffs, pets, light pets, and buffs without a time display (minions/auras)
            if (Main.debuff[i] || 
                Main.buffNoTimeDisplay[i] || 
                Main.lightPet[i] || 
                Main.vanityPet[i] || 
                Main.persistentBuff[i])
            {
                continue;
            }

            // --- FILTER 2: Direct ID Banlist ---
            if (BannedBuffIDs.Contains(i))
            {
                continue;
            }

            // --- FILTER 3: String Name Banlist ---
            bool isBannedByName = false;
            
            // A. Check against Modded Internal Names (e.g., "MyMod/MyBuff" or just "MyBuff")
            ModBuff modBuff = BuffLoader.GetBuff(i);
            if (modBuff != null)
            {
                string fullInternalName = $"{modBuff.Mod.Name}/{modBuff.Name}";
                if (BannedBuffNames.Contains(fullInternalName) || BannedBuffNames.Contains(modBuff.Name))
                {
                    isBannedByName = true;
                }
            }

            // B. Check against the localized Display Name (e.g., "Hellfire")
            string displayName = Lang.GetBuffName(i);
            if (!string.IsNullOrEmpty(displayName) && BannedBuffNames.Contains(displayName))
            {
                isBannedByName = true;
            }

            if (isBannedByName)
            {
                continue;
            }

            // If it passed all filters, it is a safe, positive buff!
            ValidPositiveBuffs.Add(i);
        }
    }
}