using System;
using System.IO;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace EquipmentEvolved.Assets.ModPrefixes.Summoner.MinionWeapons.Monarch;

public class MonarchGlobalProjectile : GlobalProjectile
{
    private static float _lastMonarchStacks;
    private static int _lastMonarchOwner = -1;
    private static uint _lastMonarchDeathTime;
    public override bool InstancePerEntity => true;
    
    public bool Monarch { get; set; }
    public float MonarchStacks { get; set; }
    public bool MonarchChild { get; set; }
    public float MonarchDamageMult { get; set; } = 1f;
    public int BaseDamage { get; set; }

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (projectile.minion)
        {
            Item heldItem = Main.LocalPlayer.HeldItem;
            if (heldItem.shoot == projectile.type) HandleMonarchSpawn(projectile, source);
        }

        HandleMonarchChildSpawn(projectile, source);
    }

    public override void PostAI(Projectile projectile)
    {
        if (Monarch)
        {
            (float damageMult, float scale, float slots) stats = GetMonarchStats(MonarchStacks);

            if (projectile.type == ProjectileID.StormTigerGem || Math.Abs(projectile.scale - stats.scale) > 0.01f) projectile.scale = stats.scale;
        }
    }

    public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
    {
        if (MonarchChild && MonarchDamageMult > 1f) modifiers.FinalDamage *= MonarchDamageMult;
    }

    public override void OnKill(Projectile projectile, int timeLeft)
    {
        if (!Monarch) return;
        _lastMonarchStacks = MonarchStacks;
        _lastMonarchOwner = projectile.owner;
        _lastMonarchDeathTime = Main.GameUpdateCount;
    }

    private void HandleMonarchChildSpawn(Projectile projectile, IEntitySource source)
    {
        if (source is EntitySource_Parent parentSource && parentSource.Entity is Projectile parentProj)
        {
            if (!parentProj.TryGetGlobalProjectile(out MonarchGlobalProjectile parentMonarch) || !parentMonarch.Monarch) return;
            
            (float damageMult, float scale, float slots) stats = GetMonarchStats(parentMonarch.MonarchStacks);

            MonarchChild = true;
            MonarchDamageMult = stats.damageMult;
            projectile.scale *= stats.scale;

            if (Main.netMode != NetmodeID.Server) Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.GoldFlame);
        }
    }

    private void HandleMonarchSpawn(Projectile projectile, IEntitySource source)
    {
        if (source is IEntitySource_WithStatsFromItem itemSource && itemSource.Item.HasPrefix(ModContent.PrefixType<PrefixMonarch>()))
        {
            Player player = Main.player[projectile.owner];
            Monarch = true;

            Projectile existingMonarch = null;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (!p.active || p.owner != player.whoAmI || p.type != projectile.type || p.whoAmI == projectile.whoAmI) continue;
                if (!p.GetGlobalProjectile<MonarchGlobalProjectile>().Monarch) continue;
                existingMonarch = p;
                break;
            }

            if (existingMonarch != null)
            {
                MonarchGlobalProjectile existingPrefix = existingMonarch.GetGlobalProjectile<MonarchGlobalProjectile>();
                float projectedSlots = existingMonarch.minionSlots + projectile.minionSlots;

                if (projectedSlots <= player.maxMinions)
                {
                    existingPrefix.MonarchStacks += 1f;
                    existingMonarch.minionSlots += projectile.minionSlots;
                    existingPrefix.UpdateMonarchStats(existingMonarch);
                }

                if (Main.netMode != NetmodeID.Server)
                {
                    for (int k = 0; k < 15; k++)
                    {
                        Dust dust = Dust.NewDustDirect(existingMonarch.position, existingMonarch.width, existingMonarch.height, DustID.GoldFlame);
                        dust.velocity *= 1.5f;
                        dust.noGravity = true;
                    }
                }

                projectile.active = false;
                projectile.Kill();
            }
            else
            {
                bool isReincarnation = _lastMonarchOwner == projectile.owner && Main.GameUpdateCount - _lastMonarchDeathTime <= 1;

                if (isReincarnation)
                {
                    float potentialNewStacks = _lastMonarchStacks + 1f;
                    MonarchStacks = potentialNewStacks > player.maxMinions ? _lastMonarchStacks : potentialNewStacks;
                }
                else
                    MonarchStacks = 1f;

                UpdateMonarchStats(projectile);
            }
        }
    }

    public static (float damageMult, float scale, float slots) GetMonarchStats(float stacks)
    {
        float effectiveStacks = stacks;
        if (stacks > PrefixBalance.MONARCH_SOFT_CAP)
        {
            float excess = stacks - PrefixBalance.MONARCH_SOFT_CAP;
            effectiveStacks = PrefixBalance.MONARCH_SOFT_CAP + excess * PrefixBalance.MONARCH_DIMINISHING_RETURN;
        }

        float growth;
        if (stacks <= PrefixBalance.MONARCH_SOFT_CAP)
            growth = (stacks - 1) * PrefixBalance.MONARCH_SIZE_PER_STACK;
        else
        {
            float baseGrowth = (PrefixBalance.MONARCH_SOFT_CAP - 1) * PrefixBalance.MONARCH_SIZE_PER_STACK;
            float excessGrowth = (stacks - PrefixBalance.MONARCH_SOFT_CAP) * (PrefixBalance.MONARCH_SIZE_PER_STACK * PrefixBalance.MONARCH_DIMINISHING_RETURN);
            growth = baseGrowth + excessGrowth;
        }

        float finalScale = PrefixBalance.MONARCH_BASE_SIZE + growth;
        float efficiencyMultiplier = 1f + (effectiveStacks - 1) * PrefixBalance.MONARCH_EFFICIENCY_BONUS_PER_STACK;
        float finalDamageMult = effectiveStacks * efficiencyMultiplier;

        float finalSlots = stacks;
        if (Main.LocalPlayer.active && finalSlots > Main.LocalPlayer.maxMinions) finalSlots = Main.LocalPlayer.maxMinions;

        return (finalDamageMult, finalScale, finalSlots);
    }

    public void UpdateMonarchStats(Projectile projectile)
    {
        (float damageMult, float scale, float slots) stats = GetMonarchStats(MonarchStacks);

        projectile.scale = stats.scale;

        if (BaseDamage == 0) BaseDamage = projectile.originalDamage;

        projectile.originalDamage = (int)(BaseDamage * stats.damageMult);

        Player player = Main.player[projectile.owner];
        float slotsIdeally = stats.slots;
        if (slotsIdeally > player.maxMinions) slotsIdeally = player.maxMinions;

        projectile.minionSlots = slotsIdeally;

        if (Main.netMode != NetmodeID.SinglePlayer) projectile.netUpdate = true;
    }

    public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
    {
        bitWriter.WriteBit(Monarch);
        binaryWriter.Write(MonarchStacks);
        bitWriter.WriteBit(MonarchChild);
        binaryWriter.Write(MonarchDamageMult);
    }

    public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
    {
        Monarch = bitReader.ReadBit();
        MonarchStacks = binaryReader.ReadSingle();
        MonarchChild = bitReader.ReadBit();
        MonarchDamageMult = binaryReader.ReadSingle();
    }
}