using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Utilities;

public static class WeaponUtils
{
    public static void FullHitCancellation(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.SetMaxDamage(1);
        modifiers.DisableCrit();
        modifiers.HideCombatText();
        //if (target.life >= target.lifeMax) return;
        //target.life++;
    }

    /// <summary>
    ///     Inflicts an ADDITIONAL separate hit of True Damage (for OnHitAnything/Effects).
    /// </summary>
    public static void InflictTrueDamage(NPC target, float damage)
    {
        if (target.type == NPCID.TargetDummy && !PrefixBalance.DEV_MODE) return;

        if (damage < 1) damage = 1;

        int dmgInt = (int)damage;
        
        NPC.HitInfo hitInfo = new()
        {
            Damage = dmgInt,
            HideCombatText = true
        };
        target.StrikeNPC(hitInfo);

        if (Main.netMode == NetmodeID.Server) return;
        
        CombatText.NewText(UtilMethods.GetCombatTextRect(target), Color.WhiteSmoke, dmgInt, true, true);
        if (Main.netMode == NetmodeID.MultiplayerClient) SendTrueDamagePacket(target, dmgInt);
    }

    private static void SendTrueDamagePacket(NPC target, int damage)
    {
        ModPacket packet = EquipmentEvolved.Instance.GetPacket();
        packet.Write((byte)MessageType.TrueDamageText);
        packet.Write((int)Color.WhiteSmoke.PackedValue);
        packet.Write(target.Center.X);
        packet.Write(target.Center.Y - 20);
        packet.Write(damage);
        packet.Write(Main.LocalPlayer.whoAmI);
        packet.Send();
    }

    public static void ShowTrueDamageText(int targetWhoAmI, int damage)
    {
        NPC target = Main.npc[targetWhoAmI];
        if (!target.active) return;

        CombatText.NewText(UtilMethods.GetCombatTextRect(target), Color.WhiteSmoke, damage, true, true);
    }

    public static void SpawnPerceptiveText(Vector2 position, int damage, int tier)
    {
        float lerpAmount = (tier - 1) / 5f;
        lerpAmount = MathHelper.Clamp(lerpAmount, 0f, 1f);

        Color startColor = Color.Orange;
        Color endColor = new(255, 30, 30);
        Color finalColor = Color.Lerp(startColor, endColor, lerpAmount);

        float damageMult = 2f + (tier > 1 ? (tier - 1) * 0.5f : 0f);
        int totalMultiplier = (int)(damageMult * 2);

        string combatString = $"{damage} (x{totalMultiplier})";

        int textIndex = CombatText.NewText(new Rectangle((int)position.X, (int)position.Y, 1, 1), finalColor, combatString, true);

        if (textIndex > -1 && textIndex < Main.combatText.Length) Main.combatText[textIndex].scale = 1.4f + tier * 0.15f;
    }
}