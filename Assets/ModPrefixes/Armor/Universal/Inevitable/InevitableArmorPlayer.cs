using System;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.Inevitable;

public class InevitableArmorPlayer : ModPlayer
{
    public int InevitablePiecesEquipped { get; set; }
    public int InevitableTimer;

    public override void ResetEffects()
    {
        InevitablePiecesEquipped = 0;
    }

    public override void UpdateEquips()
    {
        for (int i = 0; i < 3; i++)
        {
            if (Player.armor[i] != null && !Player.armor[i].IsAir && Player.armor[i].prefix == ModContent.PrefixType<PrefixInevitable>())
            {
                InevitablePiecesEquipped++;
                Player.GetDamage(DamageClass.Generic) += PrefixBalance.INEVITABLE_PIECE_DAMAGE_MULT;
            }
        }
    }

    public override void PostUpdateEquips()
    {
        if (InevitablePiecesEquipped == 3)
        {
            Player.GetModPlayer<ArmorAbilityPlayer>().SetArmorAbility(ActivateInevitable);
        }
        
        if (InevitableTimer > 0)
        {
            InevitableTimer--;
            
            if (InevitableTimer % 60 == 0)
            {
                SoundEngine.PlaySound(SoundID.MenuTick, Player.Center);
            }
            
            if (Main.rand.NextBool(4))
            {
                Vector2 randomOffset = Main.rand.NextVector2Circular(60f, 60f);
                Dust dust = Dust.NewDustPerfect(Player.Center + new Vector2(0, -40) + randomOffset, DustID.MagicMirror, Vector2.Zero, 0, Color.Cyan, 1.2f);
                dust.noGravity = true;
            }
            
            if (InevitableTimer == 0)
            {
                ExecuteWipe();
            }
        }
    }

    private int ActivateInevitable()
    {
        InevitableTimer = PrefixBalance.INEVITABLE_CLOCK_TICKS;
        SoundEngine.PlaySound(SoundID.Item4, Player.Center);
        
        return PrefixBalance.INEVITABLE_COOLDOWN_TICKS;
    }

    private void ExecuteWipe()
    {
        SoundEngine.PlaySound(SoundID.Item119, Player.Center);
        
        for (int i = 0; i < 100; i++)
        {
            Vector2 velocity = Main.rand.NextVector2Circular(25f, 25f);
            Dust dust = Dust.NewDustPerfect(Player.Center, DustID.Vortex, velocity, 0, Color.White, 2.5f);
            dust.noGravity = true;
        }

        foreach (NPC npc in Main.ActiveNPCs)
        {
            if (npc.active && !npc.friendly && !npc.dontTakeDamage && npc.lifeMax > 5)
            {
                if (Vector2.Distance(Player.Center, npc.Center) <= PrefixBalance.INEVITABLE_RANGE)
                {
                    int damage;

                    if (npc.boss)
                    {
                        damage = (int)(npc.lifeMax * PrefixBalance.INEVITABLE_BOSS_MAX_HP_DAMAGE_PERCENT);
                    }
                    else
                    {
                        damage = npc.lifeMax * 5; 
                    }
                    
                    NPC.HitInfo hit = npc.CalculateHitInfo(damage, Math.Sign(npc.Center.X - Player.Center.X), false, 0f, DamageClass.Generic);
                    
                    hit.Damage = damage; 
                    
                    Player.StrikeNPCDirect(npc, hit);
                    
                    for (int i = 0; i < 15; i++)
                    {
                        Dust.NewDust(npc.position, npc.width, npc.height, DustID.Vortex, 0f, 0f, 100, default, 1.5f);
                    }
                }
            }
        }
    }
}