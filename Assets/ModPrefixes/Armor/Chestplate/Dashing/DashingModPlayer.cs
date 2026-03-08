using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Chestplate.Dashing;

public class DashingModPlayer : ModPlayer
{
    public int Charges = 3;
    public int DashDuration;
    public Vector2 DashVelocity;
    public float FadeAlpha;

    public int FadeDelay;
    public bool IsDashingEquipped;
    public int RechargeTimer;

    public override void ResetEffects()
    {
        IsDashingEquipped = false;
    }

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (!IsDashingEquipped || Charges <= 0 || DashDuration > 0) return;

        if (!ArmorKeybindSystem.ArmorActivationKeybind.JustPressed) return;

        Charges--;
        DashDuration = PrefixBalance.DASHING_DURATION_TICKS;

        DashVelocity = (Main.MouseWorld - Player.Center).SafeNormalize(Vector2.Zero) * PrefixBalance.DASHING_VELOCITY;

        FadeAlpha = 1f;
        FadeDelay = 120;

        SoundEngine.PlaySound(SoundID.Item28, Player.Center);
    }

    public override void PostUpdateRunSpeeds()
    {
        if (!IsDashingEquipped)
        {
            Charges = PrefixBalance.DASHING_MAX_CHARGES;
            return;
        }

        if (Charges < PrefixBalance.DASHING_MAX_CHARGES)
        {
            RechargeTimer++;
            if (RechargeTimer >= PrefixBalance.DASHING_RECHARGE_TICKS)
            {
                RechargeTimer = 0;
                Charges++;
                if (Charges == PrefixBalance.DASHING_MAX_CHARGES) FadeDelay = 120;
            }
        }

        if (DashDuration > 0)
        {
            Player.velocity = DashVelocity;

            Player.gravity = 0f;
            Player.ignoreWater = true;

            Player.maxFallSpeed = 200f;
            Player.armorEffectDrawShadow = true;
            Player.noFallDmg = true;
            DashDuration--;
        }

        if (Charges < PrefixBalance.DASHING_MAX_CHARGES || DashDuration > 0)
        {
            FadeAlpha = 1f;
            FadeDelay = 120;
        }
        else
        {
            if (FadeDelay > 0)
                FadeDelay--;
            else if (FadeAlpha > 0) FadeAlpha -= 0.05f;
        }
    }
}