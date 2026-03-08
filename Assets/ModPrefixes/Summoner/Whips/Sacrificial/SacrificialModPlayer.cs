using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Summoner.Whips.Sacrificial;

public class SacrificialModPlayer : ModPlayer
{
    public int SacrificialCooldown { get; set; }

    public override void OnHitAnything(float x, float y, Entity victim)
    {
        if (Player.HeldItem.HasPrefix(ModContent.PrefixType<PrefixSacrificial>())) SacrificialHit();
    }

    public override void PostUpdateEquips()
    {
        SacrificialCooldown--;
    }

    private void SacrificialHit()
    {
        if (SacrificialCooldown > 0) return;

        Player.AddBuff(ModContent.BuffType<SacrificialBuff>(), PrefixBalance.SACRIFICIAL_ON_HIT_BUFF_TICKS);
    }

    public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
    {
        if (Player.HasBuff<SacrificialBuff>())
        {
            Player.immuneTime = Player.numMinions * PrefixBalance.SACRIFICIAL_IMMUNE_FRAMES_PER_MINION;
            Player.numMinions = 0;
            Player.ClearBuff(ModContent.BuffType<SacrificialBuff>());

            SacrificialCooldown = PrefixBalance.SACRIFICIAL_COOLDOWN_TICKS;

            return false;
        }

        return true;
    }
}