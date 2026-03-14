using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Core;

/// <summary>
/// Determines how multiple sources of the same stat contribute to a final value.
/// The stacking mode affects how these contributions are combined, influencing
/// the nature of their interaction and the resulting output value.
/// </summary>
public enum StatStackingMode
{
    /// <summary>
    /// The <see cref="StatStackingMode.Additive"/> mode combines stat values by summing them together.
    /// This is commonly used when the contribution of each individual source to a stat is intended to
    /// linearly aggregate, increasing the total value without modifiers like scaling or diminishing returns.
    /// </summary>
    Additive,
    /// <summary>
    /// The <see cref="StatStackingMode.Multiplicative"/> mode combines stat values by multiplying them together.
    /// This approach results in an exponential scaling of the stat, where each contributing source acts as a
    /// multiplier, commonly used to create compounded effects or progressive increases.
    /// </summary>
    Multiplicative,
    /// <summary>
    /// The <see cref="StatStackingMode.Asymptotic"/> mode combines stat values in a manner that approaches
    /// a defined limit as contributions from additional sources increase. This is often used to model effects
    /// with diminishing returns, where each successive contribution provides less incremental value, ensuring
    /// the stat does not grow infinitely.
    /// </summary>
    Asymptotic,
    /// <summary>
    /// The <see cref="StatStackingMode.Max"/> mode selects the highest stat value among all sources.
    /// This is often used when only the most significant contributor should be considered, ignoring
    /// all other lower values completely.
    /// </summary>
    Max,
    /// <summary>
    /// The <see cref="StatStackingMode.Min"/> mode combines stat values by selecting the smallest value
    /// among all contributing sources. This is typically used when the lowest value provides the most constraint
    /// or restriction, ensuring that only the minimum value affects the final outcome.
    /// </summary>
    Min
}

public abstract class EquipmentStat : ModType
{
    public int NetID { get; internal set; }
    
    public virtual StatStackingMode StackingMode => StatStackingMode.Additive;

    public abstract string FormatTooltip(float totalValue);

    // =================================================================
    // CORE PASSIVE HOOKS (Health, Defense, MoveSpeed, WingTime, Minions)
    // =================================================================
    public virtual void UpdateEquips(Player player, float totalValue) { }
    public virtual void PostUpdateEquips(Player player, float totalValue) { }
    public virtual void UpdateLifeRegen(Player player, float totalValue) { }
    public virtual void ModifyMaxStats(Player player, ref StatModifier health, ref StatModifier mana, float totalValue) { }

    // =================================================================
    // OFFENSIVE COMBAT HOOKS (Damage, Crit, Speed, Mana)
    // =================================================================
    public virtual void ModifyWeaponDamage(Player player, Item item, ref StatModifier damage, float totalValue) { }
    public virtual void ModifyWeaponCrit(Player player, Item item, ref float crit, float totalValue) { }
    public virtual void ModifyWeaponKnockback(Player player, Item item, ref StatModifier knockback, float totalValue) { }
    public virtual void ModifyManaCost(Player player, Item item, ref float reduce, ref float mult, float totalValue) { }
    
    public virtual float UseSpeedMultiplier(Player player, Item item, float totalValue) => 1f;

    // =================================================================
    // ON-HIT / HIT MODIFIER HOOKS (True Damage, Lifesteal, Coin Drops)
    // =================================================================
    public virtual void ModifyHitNPCWithProj(Player player, Projectile proj, NPC target, ref NPC.HitModifiers modifiers, float totalValue) { }
    
    public virtual void OnHitNPCWithProj(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone, float totalValue) { }
    
    // 1. Generic Catch-All Hits (No Item Context)
    public virtual void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers, float totalValue) { }
    public virtual void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone, float totalValue) { }

    // 2. Physical Melee Swings (Has Item Context)
    public virtual void ModifyHitNPCWithItem(Player player, Item item, NPC target, ref NPC.HitModifiers modifiers, float totalValue) { }
    public virtual void OnHitNPCWithItem(Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone, float totalValue) { }

    // =================================================================
    // DEFENSIVE / HURT HOOKS (Damage Reduction, Iframes, Dodges)
    // =================================================================
    public virtual void ModifyHurt(Player player, ref Player.HurtModifiers modifiers, float totalValue) { }
    public virtual void OnHurt(Player player, Player.HurtInfo info, float totalValue) { }
    public virtual void PostHurt(Player player, Player.HurtInfo info, float totalValue) { }
    
    // Returns true if the stat successfully dodged the attack
    public virtual bool FreeDodge(Player player, Player.HurtInfo info, float totalValue) => false;

    // =================================================================
    // MOBILITY / UTILITY HOOKS (Wing speeds, Pickaxe speed, Healing)
    // =================================================================
    public virtual void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration, float totalValue) { }
    public virtual void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend, float totalValue) { }
    public virtual void GetHealLife(Player player, Item item, bool quickHeal, ref int healValue, float totalValue) { }
    
    public virtual bool CanUseItem(Player player, Item item, float totalValue) => true;

    protected override void Register()
    {
        ModTypeLookup<EquipmentStat>.Register(this);
        EquipmentStatLoader.RegisterStat(this);
    }
}