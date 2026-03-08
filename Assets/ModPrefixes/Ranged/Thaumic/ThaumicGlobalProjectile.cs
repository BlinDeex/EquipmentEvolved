using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Thaumic;

public class ThaumicGlobalProjectile : GlobalProjectile
{
    public bool IsThaumic;
    public override bool InstancePerEntity => true;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source is IEntitySource_WithStatsFromItem itemSource && itemSource.Item.HasPrefix(ModContent.PrefixType<PrefixThaumic>())) IsThaumic = true;
    }

    public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (!IsThaumic) return;

        Player player = Main.player[projectile.owner];

        if (ThaumicBuffManager.ValidPositiveBuffs.Count == 0) return;

        HashSet<int> activeBuffs = new();
        for (int i = 0; i < player.CountBuffs(); i++)
        {
            if (player.buffTime[i] > 0) activeBuffs.Add(player.buffType[i]);
        }

        List<int> missingBuffs = [];
        List<int> activeValidBuffs = [];

        foreach (int buffID in ThaumicBuffManager.ValidPositiveBuffs)
        {
            if (activeBuffs.Contains(buffID))
                activeValidBuffs.Add(buffID);
            else
                missingBuffs.Add(buffID);
        }

        int buffToApply = -1;
        bool extending = false;

        if (missingBuffs.Count > 0)
            buffToApply = missingBuffs[Main.rand.Next(missingBuffs.Count)];
        else if (activeValidBuffs.Count > 0)
        {
            buffToApply = activeValidBuffs[Main.rand.Next(activeValidBuffs.Count)];
            extending = true;
        }
        
        if (buffToApply == -1) return;
        
        if (extending)
        {
            int buffIndex = player.FindBuffIndex(buffToApply);
            if (buffIndex == -1) return;
            
            player.buffTime[buffIndex] += PrefixBalance.THAUMIC_EXTENSION_TICKS;
            if (player.buffTime[buffIndex] > PrefixBalance.THAUMIC_MAX_DURATION_TICKS) player.buffTime[buffIndex] = PrefixBalance.THAUMIC_MAX_DURATION_TICKS;
        }
        else
            player.AddBuff(buffToApply, PrefixBalance.THAUMIC_BASE_DURATION_TICKS);
    }
}