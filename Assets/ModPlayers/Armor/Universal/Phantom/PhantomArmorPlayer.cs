using System;
using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Buffs;
using EquipmentEvolved.Assets.CharmsModule.Augmentations;
using EquipmentEvolved.Assets.Projectiles;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPlayers.Armor.Universal.Phantom;

public class PhantomArmorPlayer : ModPlayer
{
    public int PhantomPiecesEquipped { get; set; }
    
    public bool PhantomSetBonus;

    private List<PhantomArmorProjectile> activePhantoms = [];

    private int oldHp = int.MaxValue;
    private int oldMana = int.MaxValue;
    
    private static readonly SoundStyle soulBurnSound = new("EquipmentEvolved/Assets/Sounds/SoulburnSound")
    {
        IsLooped = true,
        MaxInstances = 1,
        SoundLimitBehavior = SoundLimitBehavior.IgnoreNew,
        Type = SoundType.Ambient 
    };
    
    private SlotId soulBurnSlot = SlotId.Invalid;
    
    public override void PostUpdateEquips()
    {
        PhantomSetBonus = PhantomPiecesEquipped == 3;
        if(PhantomSetBonus) Player.GetModPlayer<ArmorAbilityPlayer>().SetArmorAbility(PhantomArmorAbility);
    }

    public override void ResetEffects()
    {
        PhantomPiecesEquipped = 0;
    }

    public override bool CanUseItem(Item item)
    {
        if (Player.HasBuff<VoidSealBuff>())
        {
            if (item.healLife > 0) return false;
            if (item.healMana > 0) return false;
        }

        return true;
    }

    public override void PostUpdateMiscEffects()
    {
        SoulBurnEffect();
        SoulBurnSound();
        if (activePhantoms.Count == 0)
        {
            int buffIndex = Player.FindBuffIndex(ModContent.BuffType<SoulBurnBuff>());
            if (buffIndex >= 0)
            {
                Player.buffTime[buffIndex] -= PrefixBalance.PHANTOM_RECOVERY_SPEED;
            }
        }
    }

    private void SoulBurnSound()
    {
        const float START_THRESHOLD = 0.01f; 
        const float MAX_VOLUME = 0.7f;       
        const float CURVE_POWER = 3.4f;      

        int buffIndex = Player.FindBuffIndex(ModContent.BuffType<SoulBurnBuff>());
        float intensity = 0f;

        if (buffIndex >= 0)
        {
            intensity = Player.buffTime[buffIndex] / 1200f;
            if (intensity > 1f) intensity = 1f;
        }

        if (intensity >= START_THRESHOLD)
        {
            if (!SoundEngine.TryGetActiveSound(soulBurnSlot, out ActiveSound activeSound))
            {
                SoundStyle startingStyle = soulBurnSound;
                
                startingStyle.Volume = 1.0f; //TODO: thump sound at the start of sound, I need to start playing at low volume but this acts like master volume
                
                soulBurnSlot = SoundEngine.PlaySound(startingStyle);
                
                if (SoundEngine.TryGetActiveSound(soulBurnSlot, out activeSound))
                {
                    activeSound.Volume = 0f; // workaround by keeping volume at 1 then immediately muting it didn't work
                }
            }
            else
            {
                float range = 1.0f - START_THRESHOLD;
                float progress = (intensity - START_THRESHOLD) / range;
                if (progress < 0) progress = 0;
                
                float curvedVolume = MathF.Pow(progress, CURVE_POWER);
                activeSound.Volume = curvedVolume * MAX_VOLUME;
            }
        }
        else
        {
            if (SoundEngine.TryGetActiveSound(soulBurnSlot, out ActiveSound activeSound))
            {
                activeSound.Stop();
            }
        }
    }

    public override void PreUpdateBuffs()
    {
        if (Player.HasBuff<VoidSealBuff>())
        {
            if (Player.statLife > oldHp) Player.statLife = oldHp;
            if (Player.statMana > oldMana) Player.statMana = oldMana;
        }
        
        oldHp = Player.statLife;
        oldMana = Player.statMana;
    }

    private int PhantomArmorAbility()
    {
        if (activePhantoms.Count > 0)
        {
            foreach (PhantomArmorProjectile activePhantom in activePhantoms)
            {
                activePhantom.InitiateCloneDestruction();
            }
            
            activePhantoms.Clear();
            
            return PrefixBalance.PHANTOM_ABILITY_COOLDOWN;
        }
        
        
        bool phantomAugmented = Player.GetModPlayer<AugmentationsPlayer>().PhantomAugmentation;
        int projectileCount = phantomAugmented ? PrefixBalance.PHANTOM_CLONES_COUNT_AUGMENTED : PrefixBalance.PHANTOM_CLONES_COUNT;
        
        float angleIncrementor = 360f / projectileCount;
        float currentIncrementor = 0;

        for (int i = 0; i < projectileCount; i++)
        {
            PhantomArmorProjectile PhantomProj = (PhantomArmorProjectile)Projectile.NewProjectileDirect(null, Player.Center, Vector2.Zero,
                ModContent.ProjectileType<PhantomArmorProjectile>(), 0, 0, Player.whoAmI).ModProjectile;
            
            PhantomProj.Initialize(currentIncrementor);
            currentIncrementor += angleIncrementor;
            activePhantoms.Add(PhantomProj);
        }

        return 0;
    }
    
    public void SoulBurnEffect()
    {
        if (Main.netMode == Terraria.ID.NetmodeID.Server || Player.whoAmI != Main.myPlayer) return;

        int buffIndex = Player.FindBuffIndex(ModContent.BuffType<SoulBurnBuff>());

        if (buffIndex >= 0)
        {
            int currentDuration = Player.buffTime[buffIndex];

            float intensity = currentDuration / 1200f;
            if (intensity > 1f) intensity = 1f;

            if (!Filters.Scene["EquipmentEvolved:SoulBurn"].IsActive())
            {
                Filters.Scene.Activate("EquipmentEvolved:SoulBurn");
            }

            Filters.Scene["EquipmentEvolved:SoulBurn"].GetShader().UseIntensity(intensity);
        }
        else
        {
            if (Filters.Scene["EquipmentEvolved:SoulBurn"].IsActive())
            {
                Filters.Scene["EquipmentEvolved:SoulBurn"].Deactivate();
            }
        }
    }
}