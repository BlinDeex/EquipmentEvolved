using System;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.Untouchable;

public class UntouchableModPlayer : ModPlayer
{
    private Item untouchableBuffedWeapon;
    private int untouchableBuffedWeaponOldAnimTime;
    private int untouchableBuffedWeaponOldUseTime;
    public float UntouchableDamageIncrease { get; set; }

    public override void ModifyHurt(ref Player.HurtModifiers modifiers)
    {
        if (UntouchableDamageIncrease > 0) modifiers.FinalDamage *= 1f + UntouchableDamageIncrease;
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (UntouchableDamageIncrease > 0 || untouchableBuffedWeapon != null) ResetUntouchable();
    }

    private void ResetUntouchable()
    {
        if (untouchableBuffedWeapon != null)
        {
            untouchableBuffedWeapon.useTime = untouchableBuffedWeaponOldUseTime;
            untouchableBuffedWeapon.useAnimation = untouchableBuffedWeaponOldAnimTime;
            untouchableBuffedWeapon = null;
        }

        UntouchableDamageIncrease = 0;
    }

    public override void PostUpdateEquips()
    {
        UntouchableTick();
    }

    private void UntouchableTick()
    {
        if (Player.HeldItem.HasPrefix(ModContent.PrefixType<PrefixUntouchable>()))
            UntouchableDamageIncrease += PrefixBalance.UNTOUCHABLE_INCREASE_PER_TICK;
        else
            UntouchableDamageIncrease = 0;

        UntouchableDamageIncrease = MathHelper.Clamp(UntouchableDamageIncrease, 0f, PrefixBalance.UNTOUCHABLE_MAX_INCREASE);

        bool maxIncrease = Math.Abs(UntouchableDamageIncrease - PrefixBalance.UNTOUCHABLE_MAX_INCREASE) < 0.01f;

        if (untouchableBuffedWeapon != null) UntouchableEffects();

        if (maxIncrease && untouchableBuffedWeapon == null)
        {
            Item heldItem = Player.HeldItem;
            untouchableBuffedWeaponOldUseTime = heldItem.useTime;
            untouchableBuffedWeaponOldAnimTime = heldItem.useAnimation;
            untouchableBuffedWeapon = heldItem;

            int newUseTime = (int)(Player.HeldItem.useTime * PrefixBalance.UNTOUCHABLE_SWING_DECREASE);
            if (newUseTime == 0) newUseTime = 1;

            //heldItem.useTime = newUseTime;
            heldItem.useAnimation = newUseTime;
            return;
        }

        if (!maxIncrease && untouchableBuffedWeapon != null)
        {
            untouchableBuffedWeapon.useTime = untouchableBuffedWeaponOldUseTime;
            untouchableBuffedWeapon.useAnimation = untouchableBuffedWeaponOldAnimTime;
            untouchableBuffedWeapon = null;
        }
    }

    private void UntouchableEffects()
    {
        Player.yoraiz0rEye = 7;
    }
}