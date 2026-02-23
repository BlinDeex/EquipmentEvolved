using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.CharmsModule.Augmentations;
using EquipmentEvolved.Assets.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPlayers.Armor.Universal;

public class CursedArmorPlayer : ModPlayer
{
    public int CursedPiecesEquipped { get; set; }
    
    public bool CursedSetBonus;
    
    public override void PostUpdateEquips()
    {
        CursedSetBonus = CursedPiecesEquipped == 3;
        if(CursedSetBonus) Player.GetModPlayer<ArmorAbilityPlayer>().SetArmorAbility(null);
    }

    public override void ResetEffects()
    {
        CursedPiecesEquipped = 0;
    }

    public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
    {
        if (!CursedSetBonus) return;
        
        modifiers.ModifyHurtInfo += (ref Player.HurtInfo info) =>
        {
            ModifyDamageForCursed(ref info);
        };
    }
    
    public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
    {
        if (!CursedSetBonus) return;
        
        modifiers.ModifyHurtInfo += (ref Player.HurtInfo info) =>
        {
            ModifyDamageForCursed(ref info);
        };
    }

    private void ModifyDamageForCursed(ref Player.HurtInfo info)
    {
        bool cursedAugmentation = Player.GetModPlayer<AugmentationsPlayer>().CursedAugmentation;
        
        float ignoredDamageThreshold = cursedAugmentation
            ? PrefixBalance.AUGMENTATION_CURSED_IGNORED_DAMAGE_PERCENT_THRESHOLD
            : PrefixBalance.CURSED_IGNORED_DAMAGE_PERCENT_THRESHOLD;
        
        float damageTakenPercent = cursedAugmentation
            ? PrefixBalance.AUGMENTATION_CURSED_DAMAGE_TAKEN_PERCENT
            : PrefixBalance.CURSED_DAMAGE_TAKEN_PERCENT;
        
        
        if (info.Damage > Player.statLifeMax2 * ignoredDamageThreshold)
        {
            info.Damage = (int)(Player.statLifeMax2 * damageTakenPercent);
        }
        else
        {
            info.Cancelled = true;
            UtilMethods.AnnounceNegated(Player);
            Player.SetImmuneTimeForAllTypes(20);
        }
    }
}