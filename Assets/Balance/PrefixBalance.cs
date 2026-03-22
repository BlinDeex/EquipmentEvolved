using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable ConvertToConstant.Global
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace EquipmentEvolved.Assets.Balance;

[SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible")]
[SuppressMessage("ReSharper", "ConvertToConstant.Local")]
public static class PrefixBalance
{
    public static bool DEV_MODE = false;

    // Reforge cost multipliers
    public static float ACCESSORY_REFORGING_MULTIPLIER = 0.7f;
    public static float WEAPON_REFORGING_MULTIPLIER = 0.7f;
    public static float TOOL_REFORGING_MULTIPLIER = 0.7f;
    public static float ARMOR_REFORGING_MULTIPLIER = 0.7f;

    // Global nerf multipliers
    public static float GLOBAL_WEAPON_DAMAGE_NERF_MUL = 0.70f;
    public static float GLOBAL_ARMOR_DEFENSE_NERF_MUL = 0.75f;

    private static float INFERNAL_DAMAGE_REQUIRED_SCALAR = 1000f;

    public static int GetInfernalDamageRequired(Item item)
    {
        float damageRequired = item.damage * (50f / item.useTime) * INFERNAL_DAMAGE_REQUIRED_SCALAR;
        damageRequired = RoundToNearestThousand(damageRequired);
        return (int)damageRequired;
    }

    private static float RoundToNearestThousand(float value)
    {
        if (value < 1000) return 1000;

        return (float)Math.Round(value / 1000) * 1000;
    }

    #region Misc

    public static readonly Color INFERNAL_MIN_COLOR = new(255, 90, 90);
    public static readonly Color INFERNAL_MAX_COLOR = new(90, 90, 255);

    #endregion

    #region MeleePrefixes

    // Gigantic
    public static float GIGANTIC_SIZE = 1.75f; // +75% size

    public static float GIGANTIC_USE = 1.15f; // 15% slower
    
    // Ultra Light
    public static float ULTRA_LIGHT_USE = 0.5f; // 2x swing speed
    public static float ULTRA_LIGHT_DAMAGE = 0.65f; // 35% less damage
    public static float ULTRA_LIGHT_SIZE = 0.85f; // 15% smaller

    // Arcane Infused
    public static float ARCANE_INFUSED_DAMAGE = 1.85f; // Massively overcomes the -30% global nerf (1.85 * 0.7 = 1.30x Vanilla DPS)

    public static float ARCANE_INFUSED_MANA_PER_SWING = 12f;

    // Titan Force
    public static float TITAN_FORCE_KB = 5f; // Enormous knockback
    public static float TITAN_FORCE_DAMAGE = 1.25f; // Covers the global damage nerf nicely

    // Perceptive
    public static int PERCEPTIVE_CRIT = 50; // +50% base crit (requires accessories to hit the 800% cap!)
    public static float PERCEPTIVE_CRIT_DAMAGE = 0.90f;
    public static int PERCEPTIVE_MAX_TIER = 6; // <-- ADD THIS HERE!

    // Untouchable
    public static float UNTOUCHABLE_INCREASE_PER_TICK = 1f / (60f * 15f); // Reaches max in 15 seconds 
    public static float UNTOUCHABLE_MAX_INCREASE = 2.5f; // +250% damage. Absolutely melts bosses if you don't get hit.
    public static float UNTOUCHABLE_SWING_DECREASE = 0.75f; // 25% faster swing at max stacks

    // Kinetic
    public static float KINETIC_DAMAGE_NERF = 0.20f; // 80% damage penalty when standing perfectly still

    public static float KINETIC_VELOCITY_DAMAGE_SCALAR = 0.25f; // Scales aggressively with fast movement (dashing, falling)

    // Sundering
    public static float SUNDERING_DAMAGE_MUL = 0.50f; // Extremely low base damage
    public static float SUNDERING_DEFENSE_REDUCTION = 0.02f; // Permanently shreds 2% enemy defense per hit

    // PAYBACK SETTINGS
    public static int PAYBACK_MIN_COINS = 1;
    public static int PAYBACK_MAX_COINS = 3;
    public static float PAYBACK_COIN_RETURN_CHANCE = 0.95f; // 95% chance to not lose the money

    // Payback Multipliers (Heavily rewards risking Platinum)
    public static float PAYBACK_COPPER_MULT = 0.10f;
    public static float PAYBACK_SILVER_MULT = 0.40f;
    public static float PAYBACK_GOLD_MULT = 1.0f;
    public static float PAYBACK_PLATINUM_MULT = 4.0f;
    
    // Gravitic Balance
    public static float GRAVITIC_BASE_PULL_RADIUS = 100f; // Starting radius at 0 useTime
    public static float GRAVITIC_RADIUS_PER_USE_TIME = 8f; // Flat radius added per useTime tick
    public static float GRAVITIC_MAX_PULL_RADIUS = 800f; 

    public static float GRAVITIC_BASE_PULL_SPEED = 1.0f; // Base pull speed
    public static float GRAVITIC_SPEED_EXPONENT = 1.1f; // Exponential multiplier (e.g. 1.05^useTime)
    public static float GRAVITIC_MAX_PULL_SPEED = 15f; 
    
    public static float GRAVITIC_VISUAL_DISTORTION_MULT = 0.005f;

    #endregion

    #region RangedPrefixes

    // Adaptable
    public static float ADAPTABLE_FIRERATE = 1.2f;

    // Controlled
    public static float CONTROLLED_BULLET_VELOCITY = 1.75f;
    public static int CONTROLLED_BURST_DURATION_TICKS = 12;
    public static int CONTROLLED_BURST_COOLDOWN_TICKS = 35;
    public static float CONTROLLED_FIRERATE = 0.15f; // Unleashes an extremely fast burst

    // Vampiric
    public static float VAMPIRIC_FIRERATE = 1.25f; // 25% SLOWER firing speed (Fixed from 0.8f!)
    public static float VAMPIRIC_LIFESTEAL = 0.5f; // 1 HP every 2 hits

    // Infernal
    public static float INFERNAL_RANGED_MAX_DAMAGE = 1.90f; // Burnout risk is high, so the damage ceiling is massive
    public static float INFERNAL_RANGED_MAX_CRIT = 40f;

    // Equalizer
    public static float EQUALIZER_PERCENT_DAMAGE = 0.003f; // 0.003% max hp per hit (Shreds high-HP modded bosses)

    // Challenger
    public static float CHALLENGER_GOOD_ORB_HIT_MULTIPLIER = 1.1f;
    public static float CHALLENGER_GOOD_ORB_PERFECT_HIT_MULTIPLIER = 1.30f;
    public static float CHALLENGER_ORB_MISS_MULTIPLIER = 0.75f;

    public static float CHALLENGER_ACTIVATION_SCORE_THRESHOLD = 2f;
    public static float CHALLENGER_DEACTIVATION_SCORE_THRESHOLD = 1.5f;

    public static float CHALLENGER_GREEN_ORB_DEFENSE = 0.03f; // 3%
    public static float CHALLENGER_GREEN_ORB_DAMAGE = 0.03f; // 3%
    public static float CHALLENGER_BLUE_ORB_CRIT = 0.05f; // 5%
    public static float CHALLENGER_BLUE_ORB_VELOCITY = 0.15f; // 15%
    public static int CHALLENGER_YELLOW_ORB_HEAL = 15;
    public static int CHALLENGER_RED_ORB_DAMAGE = 35;

    // Thaumic
    public static int THAUMIC_BASE_DURATION_TICKS = 300; // 5 seconds
    public static int THAUMIC_EXTENSION_TICKS = 180; // 3 seconds per hit
    public static int THAUMIC_MAX_DURATION_TICKS = 1800; // 30 second cap

    // Flow State
    public static int FLOW_STATE_DURATION_TICKS = 60; // 1 second
    public static float FLOW_STATE_DPS_PERCENT_PER_STACK = 0.15f; 
    public static int FLOW_STATE_DECAY_RATE_TICKS = 2;

    // Delayed
    public static int DELAYED_PULSE_TIMER = 180; // 3 seconds
    public static float DELAYED_LASER_DAMAGE_MULT = 0.33f;

    // Ricochet
    public static int RICOCHET_MAX_SPLITS = 2;
    public static int RICOCHET_PROJECTILES_PER_SPLIT = 2;
    public static float RICOCHET_DAMAGE_MULTIPLIER_PER_SPLIT = 0.70f; // 70% damage retention per split
    public static float RICOCHET_SCALE_MULTIPLIER_PER_SPLIT = 0.75f;
    public static float RICOCHET_SPREAD_DEGREES = 30f;

    private static readonly (float, int)[] ScoreToSpawnRate =
    [
        (1f, 240), (2f, 200), (2.5f, 180), (3f, 160), (3.5f, 145), (4f, 120),
        (4.5f, 105), (5f, 95), (5.5f, 85), (6f, 80), (6.5f, 75), (7f, 70),
        (7.5f, 60), (8f, 45), (9f, 30), (9.5f, 30), (float.MaxValue, 45)
    ];

    public static int GetChallengerSpawnRate(float score)
    {
        return ScoreToSpawnRate.First(x => x.Item1 >= score).Item2;
    }

    public static float CHALLENGER_FRAME_RATE_MULTIPLIER = 0.4f;
    public static float CHALLENGER_BASELINE_ORB_SPEED = 1f;
    public static float CHALLENGER_ORB_SPEED_SCORE_PERCENTAGE_MULTIPLIER = 0.35f;

    #endregion

    #region MagicPrefixes

    // Endless
    public static float ENDLESS_MANA_USAGE = 0f;

    public static float ENDLESS_DAMAGE = 0.85f;

    // Tidal
    public static float TIDAL_MAX_DAMAGE_GAIN = 1.0f; // +100% damage at max mana

    // Inverted
    public static int INVERTED_EXPLOSION_DUST_ID = 206;

    public static float INVERTED_MAX_DAMAGE_INCREASE = 1.25f; // +125% damage, incredibly powerful but drains life quickly

    public static float INVERTED_MANA_SURGE_CHANCE_TO_DAMAGE = 0.025f;
    public static int INVERTED_MANA_SURGE_DAMAGE = 15;
    public static int INVERTED_MANA_SURGE_DUST_RATE = 2;
    public static int INVERTED_MANA_SURGE_DUST_ID = 86;
    public static float INVERTED_MANA_SURGE_MAX_SCALE = 1f;

    // Splintering
    public static int SPLINTERING_DUST_ID = 206;
    public static int SPLINTERING_MIN_SHARD_NODES = 1;
    public static int SPLINTERING_MAX_SHARD_NODES = 4;
    public static int SPLINTERING_SHARD_NODES_CRIT_MULT = 2;
    public static int SPLINTERING_MIN_NODE_PROJ = 1;
    public static int SPLINTERING_MAX_NODE_PROJ = 3;
    public static float SPLINTERING_PROJ_VELOCITY = 10f;
    public static float SPLINTERING_PROJ_VELOCITY_VARIATION_MUL = 4f;
    public static float SPLINTERING_CHANCE = 0.30f; // 30% chance

    // Chaotic
    public static int CHAOTIC_ROLL_LENGTH = 30;
    public static float CHAOTIC_COMMON_ROLL_CHANCE = 10f;
    public static float CHAOTIC_RARE_ROLL_CHANCE = 5f;
    public static float CHAOTIC_EPIC_ROLL_CHANCE = 3f;
    public static float CHAOTIC_LEGENDARY_ROLL_CHANCE = 1.5f;
    public static float CHAOTIC_NEGATIVE_ROLL_CHANCE = 9f;

    public static readonly Color CHAOTIC_COMMON_ROLL_COLOR = Color.LightGray;
    public static readonly Color CHAOTIC_RARE_ROLL_COLOR = Color.LightBlue;
    public static readonly Color CHAOTIC_EPIC_ROLL_COLOR = Color.Purple;
    public static readonly Color CHAOTIC_LEGENDARY_ROLL_COLOR = Color.YellowGreen;
    public static readonly Color CHAOTIC_NEGATIVE_ROLL_COLOR = Color.Red;

    // Triple Shot
    public static float TRIPLE_SHOT_DEGREES = 10f;
    public static float TRIPLE_SHOT_DEGREES_VARIATION = 5f;
    public static float TRIPLE_SHOT_USE_TIME_MUL = 1.0f; // Left at 1.0 so fire rate remains unchanged
    public static float TRIPLE_SHOT_DAMAGE_MUL = 0.60f; 
    public static float TRIPLE_SHOT_MANA_MUL = 3.5f;

    // Overloaded
    public static float OVERLOADED_MANA_COST_INCREASE_PER_SECOND = 1.0f;
    public static float OVERLOADED_DAMAGE_EFFICIENCY = 2.0f; // Vastly rewards committing to the full charge
    public static float OVERLOADED_MAX_SCALE_MULT = 4f;

    #endregion

    #region SummonerPrefixes

    #region MinionWeapons

    // Frenzied
    public static int FRENZIED_ADDITIONAL_UPDATES = 3; // 4x speed total

    // Monarch
    public static float MONARCH_BASE_SIZE = 0.7f;
    public static float MONARCH_SIZE_PER_STACK = 0.68f;

    public static float MONARCH_EFFICIENCY_BONUS_PER_STACK = 0.35f; // Scales incredibly hard with dedicated summoner builds

    public static int MONARCH_SOFT_CAP = 10;
    public static float MONARCH_DIMINISHING_RETURN = 0.2f;

    // Proxy
    public static int PROXY_MAX_DURATION_TICKS = 600; // 10 seconds before they dissipate
    public static float PROXY_DAMAGE_EFFICIENCY = 0.60f; // Replicated projectiles deal 60% of (Weapon + Minion) damage

    #endregion

    #region Whips

    // Sacrificial
    public static int SACRIFICIAL_ON_HIT_BUFF_TICKS = 300;
    public static int SACRIFICIAL_IMMUNE_FRAMES_PER_MINION = 30; // 0.5 seconds of invincibility per minion
    public static int SACRIFICIAL_COOLDOWN_TICKS = 3600;
    
    // Detonating
    public static float DETONATING_DAMAGE_MULTIPLIER = 3.0f; // Explosion deals 300% of the minion's base damage
    public static float DETONATING_EXPLOSION_RADIUS = 120f; // Radius in pixels
    public static float DETONATING_USE_SPEED_MUL = 1.15f;
    public static int DETONATING_AUTO_EXPLODE_TICKS = 300; // 5 seconds

    #endregion

    #endregion

    #region Accessories

    // These carry the weight of the disabled Vanilla prefixes!
    // Warlord
    public static float WARLORD_ADDITIONAL_MINIONS = 1.0f; // Flat +1 minion per accessory

    // Fortified
    public static float FORTIFIED_INCREASED_DEFENSE = 0.10f; // 10% total defense multiplier per accessory

    // Revitalizing
    public static int REVITALIZING_REGENERATION = 4; // 2hp/s regen

    // Aerodynamic
    public static float AERODYNAMIC_MOVEMENT_MULTIPLIER = 0.08f; // 8% increase
    public static int AERODYNAMIC_WING_TIME_TICKS = 45;

    // Sharpshooter
    public static int SHARPSHOOTER_CRIT = 8; // 8% base crit

    // Efficient
    public static float EFFICIENT_MANA_SAVED = 0.08f; // 8% reduced mana

    // EarthShaper
    public static float EARTH_SHAPER_PICK_SPEED_REDUCE = 0.15f; // 15% increase

    // Equilibrium
    public static float EQUILIBRIUM_MOVEMENT_MULTIPLIER = 0.03f;
    public static int EQUILIBRIUM_WING_TIME_TICKS = 15;
    public static int EQUILIBRIUM_REGENERATION = 2; // 1hp/s regen
    public static float EQUILIBRIUM_MINIONS = 0.25f;
    public static float EQUILIBRIUM_DEFENSE = 0.04f; // 4% increase

    // Risky (A fully committed 5-accessory Risky build yields +90% Damage and -60% Defense)
    public static float RISKY_DEFENSE_DECREASE = -0.12f; // -12%
    public static float RISKY_DAMAGE_INCREASE = 0.18f; // +18%

    // Stoic
    public static float STOIC_DEFENSE_INCREASE = 0.18f; // +18%
    public static float STOIC_DAMAGE_DECREASE = 0.10f; // -10%

    public static float BLOOD_FORGED_DEFENSE = 0.18f; // +25% Damage
    public static float BLOOD_FORGED_MAX_HEALTH = -0.10f; // -15% Max HP

    // Lethal
    public static float LETHAL_CRIT_DMG_MUL = 0.15f; // +15% Crit Damage

    // Hollow
    public static float HOLLOW_MAX_HEALTH_MULT = 0.80f; // -20% Max HP
    public static float HOLLOW_MOBILITY_MULT = 1.20f; // +20% Mobility

    // Lucky
    public static float LUCKY_CHARM_LUCK_MULT = 1.30f; // +30% Luck

    // Zealous
    public static float ZEALOUS_ATTACK_SPEED_MULT = 1.15f; // +15% Attack Speed

    // Greedy
    public static float GREEDY_COIN_DROP_MULT = 1.50f; // +50% Coins

    #endregion

    #region Tools

    // Clearing
    public static float CLEARING_CHANCE_TO_LOSE_MINED_BLOCK = 0.60f;
    public static float CLEARING_MINING_AREA = 3;

    // Vein miner
    //public static int VEIN_MINER_MAXIMUM_BLOCKS_MINED = 50; // currently using pickaxe power as the cap
    public static float VEIN_MINER_MINING_SPEED = 0.25f; // slower swing to compensate for mining 50 blocks at once

    // Fortune
    public static float FORTUNE_CHANCE_FOR_EXTRA_DROPS = 0.75f;
    public static int FORTUNE_MAX_EXTRA_DROPS = 4;

    // Revealing
    public static int REVEALING_TICKS = 30;
    public static float REVEALING_CHANCE = 0.25f;
    public static float REVEALING_BRIGHTNESS_MUL = 0.60f;

    // Geological
    public static float GEOLOGICAL_GEM_DROP_CHANCE = 0.05f; // 5% chance
    public static int[] GEOLOGICAL_VALID_TILES = [TileID.Dirt, TileID.Stone];

    public static int[] GEOLOGICAL_GEM_ITEM_IDS =
    [
        ItemID.Amethyst, ItemID.Topaz, ItemID.Sapphire, ItemID.Emerald,
        ItemID.Ruby, ItemID.Diamond, ItemID.Amber
    ];

    #endregion

    #region Armor

    // Phalanx
    public static int PHALANX_MIN_DAMAGE_TO_REACT = 10;
    public static int PHALANX_BURN_EFFECT_TICKS = 180;
    public static int PHALANX_REACT_COOLDOWN_TICKS = 60 * 30;
    public static float PHALANX_DAMAGE_INCREASE = 1.08f; // 8% increase per piece (+24% total)
    // Phalanx Augmentation
    public static int PHALANX_AUGMENTATION_OVERCLOCK_TICKS = 120; // 2 seconds of rapid-fire reflection
    
    // Inevitable
    public static int INEVITABLE_CLOCK_TICKS = 720; // 12 seconds
    public static float INEVITABLE_BOSS_MAX_HP_DAMAGE_PERCENT = 0.10f; // 10% max HP true damage to bosses
    public static int INEVITABLE_COOLDOWN_TICKS = 60 * 180; // 3 minute cooldown
    public static float INEVITABLE_RANGE = 2000f; // Range of the screen wipe
    public static float INEVITABLE_PIECE_DAMAGE_MULT = 0.08f; // +8% damage per piece

    #region Specialized

    // Vitalis
    public static float VITALIS_LIFESTEAL_AMP = 1.5f; // +100% lifesteal/potion healing efficiency

    // Reinforced
    public static float REINFORCED_DEFENSE_AMP = 0.50f; // +50% defense for this specific piece

    // Leggings Reforges
    public static int SILENT_AGGRO_REDUCTION = 500;
    public static float TERRA_DEFENSE_MULT = 1.25f; // +25% boost to the item's defense while grounded
    public static float DESPERATE_MAX_SPEED_BONUS = 0.60f;

    // Chestplate
    public static float VOID_TRUE_DAMAGE_BONUS = 0.30f; // +30% True Damage
    public static float UNYIELDING_MAX_DR = 0.20f; // Up to 20% Damage Reduction near 0 HP

    // Rebounding
    public static float REBOUNDING_DEFENSE_SCALING = 3.0f; // Shrapnel deals 300% of Defense as damage
    public static float REBOUNDING_DEFENSE_PER_STACK = 0.33f; // +12 Defense per hit taken
    public static int REBOUNDING_MAX_STACKS = 5;
    public static int REBOUNDING_STACK_DURATION = 300;

    // Enraging
    public static int ENRAGING_AGGRO_INCREASE = 500;

    // Berserker
    public static float BERSERKER_MAX_DAMAGE_BONUS = 0.40f; // Up to 40% extra damage near 0 HP

    // Weak Link
    public static float WEAK_LINK_THIS_DEFENSE_MULT = 0.50f; // Loses 50% of its own defense
    public static float WEAK_LINK_OTHER_DEFENSE_MULT = 1.40f; // Boosts Chest and Leggings defense by a massive 40%

    // Psionic
    public static float PSIONIC_HEAL_BONUS_MULT = 2.00f; // Hearts and stars heal 100% more
    
    // Phasing
    public static int PHASING_DASH_IFRAMES = 30; // 0.5 seconds of invincibility
    public static int PHASING_TELEPORT_IFRAMES = 60; // 1 second of invincibility

    // Dashing
    public static int DASHING_MAX_CHARGES = 3;
    public static int DASHING_RECHARGE_TICKS = 180;
    public static float DASHING_VELOCITY = 20f;
    public static int DASHING_DURATION_TICKS = 20;

    // Resonant
    public static float RESONANT_PREFIX_BOOST = 0.33f; // +33% to accessory prefixes
    
    // Quantum
    public static float QUANTUM_TELEPORT_DISTANCE = 250f;
    public static float QUANTUM_AFTERIMAGE_DEFENSE_MULT = 3.5f; //
    public static int QUANTUM_DASH_COOLDOWN_TICKS = 60; // 1 second
    public static int QUANTUM_FORGIVING_SEARCH_RADIUS_BLOCKS = 3;
    public static int QUANTUM_AFTERIMAGE_WAIT_TICKS = 30; // Half a second before moving
    public static float QUANTUM_AFTERIMAGE_RETURN_SPEED = 35f;
    public static float QUANTUM_AFTERIMAGE_MERGE_DISTANCE = 40f;
    public static Color QUANTUM_AFTERIMAGE_COLOR = new(0, 255, 255, 100);

    #endregion Specialized

    // Chrono
    public static float CHRONO_MOVEMENT_SPEED = 1.08f; // +8% per piece
    public static int CHRONO_ABILITY_COOLDOWN = 60 * 60;
    public static float CHRONO_ABILITY_RANGE = 400000f;
    public static int CHRONO_ABILITY_LENGTH = 60 * 7;

    // Phantom
    public static int PHANTOM_ABILITY_COOLDOWN = 60 * 20; //TODO soulburn doesnt deal damage to the player
    public static int PHANTOM_CLONES_COUNT = 2;
    public static int PHANTOM_CLONES_COUNT_AUGMENTED = 6;
    public static int PHANTOM_RECOVERY_SPEED = 5;
    public static float PHANTOM_TRUE_DAMAGE_AMP = 0.15f;
    // Phantom Augmentation
    public static int PHANTOM_AUGMENTATION_SOULBURN_RAMP_MULT = 2; // Soulburn builds up 2x as fast per phantom

    // Conduit
    public static int CONDUIT_MANA_PER_PIECE = 40; // Total of +120 Mana with the full set
    public static float CONDUIT_SET_MANA_REGEN_PER_TICK = 1f;

    // Raid Boss
    public static float RAID_BOSS_PIECE_DEFENSE_MULT = 1.40f; // +40% defense from this specific item
    public static float RAID_BOSS_MOBILITY_PENALTY = 0.50f; // -50% mobility
    public static int RAID_BOSS_SET_DEFENSE_BONUS = 25; // +25 flat total defense
    public static float RAID_BOSS_SET_DR_BONUS = 0.15f; // +15% Damage Reduction
    public static int RAID_BOSS_SET_AGGRO = 1000;

    // Augmented
    public static List<(int threshold, int buffID)> AUGMENTED_HELMET_THRESHOLDS_AND_BUFFS =
    [
        (5, BuffID.Hunter),
        (10, BuffID.WellFed3),
        (15, BuffID.Lifeforce)
    ];

    public static List<(int threshold, int buffID)> AUGMENTED_CHESTPLATE_THRESHOLDS_AND_BUFFS =
    [
        (5, BuffID.Ironskin),
        (10, BuffID.Wrath),
        (15, BuffID.Endurance)
    ];

    public static List<(int threshold, int buffID)> AUGMENTED_LEGGINGS_THRESHOLDS_AND_BUFFS =
    [
        (5, BuffID.Swiftness),
        (10, BuffID.Rage),
        (15, BuffID.Gravitation)
    ];

    public static List<Color> AUGMENTED_TOOLTIP_COLORS =
    [
        new(100, 255, 100),
        new(0, 255, 200),
        new(100, 100, 255)
    ];

    public static Color AUGMENTED_RED_COLOR = new(255, 100, 100);

    public static float AUGMENTED_SET_BONUS_DAMAGE_PER_BUFF = 0.02f; // +2% Damage per active buff

    // Cursed
    public static float CURSED_IGNORED_DAMAGE_PERCENT_THRESHOLD = 0.05f;
    public static float CURSED_DAMAGE_TAKEN_PERCENT = 0.8f;
    public static float CURSED_FLAT_LIFESTEAL = 0.25f;
    public static float AUGMENTATION_CURSED_IGNORED_DAMAGE_PERCENT_THRESHOLD = 0.2f;
    public static float AUGMENTATION_CURSED_DAMAGE_TAKEN_PERCENT = 0.6f;

    #endregion
}