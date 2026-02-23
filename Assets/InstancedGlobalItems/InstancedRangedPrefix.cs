using System.Collections.Generic;
using System.IO;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.ModPlayers;
using EquipmentEvolved.Assets.ModPrefixes.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace EquipmentEvolved.Assets.InstancedGlobalItems;

public class InstancedRangedPrefix : GlobalItem
{
    public override bool InstancePerEntity => true;

    public int CurrentBurstCooldown { get; private set; }

    public float DamageDone { get; set; }
    public float DamageDoneRequired { get; private set; }

    public float DamageAdded { get; private set; }
    public float CritAdded { get; private set; }
    
    public bool AscendantBurnOut { get; private set; }


    public override void SaveData(Item item, TagCompound tag)
    {
        if (DamageDone != 0)
            tag.Add(new KeyValuePair<string, object>(nameof(DamageDone), DamageDone));
        if (DamageDoneRequired != 0)
            tag.Add(new KeyValuePair<string, object>(nameof(DamageDoneRequired), DamageDoneRequired));
    }

    public override void LoadData(Item item, TagCompound tag)
    {
        DamageDone = tag.TryGet(nameof(DamageDone), out float damage) ? damage : 0;
        DamageDoneRequired = tag.TryGet(nameof(DamageDoneRequired), out float damageDoneRequired)
            ? damageDoneRequired
            : 0;
        
        CheckAscendantBurnOut();
    }

    public override void NetSend(Item item, BinaryWriter writer)
    {
        bool dontRead = DamageDone == 0 && DamageDoneRequired == 0;
        writer.Write(dontRead);

        if (dontRead) return;

        writer.Write(DamageDone);
        writer.Write(DamageDoneRequired);
    }

    public override void NetReceive(Item item, BinaryReader reader)
    {
        bool dontRead = reader.ReadBoolean();

        if (dontRead) return;

        DamageDone = reader.ReadSingle();
        DamageDoneRequired = reader.ReadSingle();
        
        CheckAscendantBurnOut();
    }

    public override bool CanStack(Item destination, Item source)
    {
        int ascendantPrefix = ModContent.PrefixType<PrefixAscendant>();
        return destination.prefix != ascendantPrefix && source.prefix != ascendantPrefix;
    }

    public override bool CanUseItem(Item item, Player player)
    {
        PrefixControlled(item, out bool canUse);
        PrefixAscendant(out canUse);
        return canUse;
    }

    private void PrefixAscendant(out bool canUse)
    {
        canUse = !AscendantBurnOut;
    }

    private void PrefixControlled(Item item, out bool canUse)
    {
        if (item.prefix == ModContent.PrefixType<PrefixControlled>())
        {
            if (CurrentBurstCooldown <= 0)
            {
                CurrentBurstCooldown = PrefixBalance.CONTROLLED_BURST_COOLDOWN_TICKS +
                                       PrefixBalance.CONTROLLED_BURST_DURATION_TICKS;
            }

            if (CurrentBurstCooldown <= PrefixBalance.CONTROLLED_BURST_COOLDOWN_TICKS)
            {
                canUse = false;
                return;
            }
        }

        canUse = true;
    }

    public override void PostReforge(Item item)
    {
        DamageDone = 0;
        DamageDoneRequired = item.prefix == ModContent.PrefixType<PrefixAscendant>()
            ? PrefixBalance.GetAscendantDamageRequired(item)
            : 0;
    }

    public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
    {
        if (item.prefix == ModContent.PrefixType<PrefixAscendant>())
        {
            float multiplier = MathHelper.Clamp(DamageDone / DamageDoneRequired, 0f, 1f);
            float damageAdded = multiplier * PrefixBalance.ASCENDANT_RANGED_MAX_DAMAGE;
            damage.Base += damageAdded * item.damage;
            DamageAdded = damageAdded;
            CheckAscendantBurnOut();
        }
    }

    private void CheckAscendantBurnOut()
    {
        if (DamageDone <= DamageDoneRequired) return;

        AscendantBurnOut = true;
    }

    public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
    {
        if (item.prefix == ModContent.PrefixType<PrefixAscendant>())
        {
            float multiplier = MathHelper.Clamp(DamageDone / DamageDoneRequired, 0f, 1f);
            float critAdded = multiplier * PrefixBalance.ASCENDANT_RANGED_MAX_CRIT;
            crit += critAdded;
            CritAdded = critAdded;
        }
    }
    
    public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation,
        ref float scale, int whoAmI)
    {
        if (!AscendantBurnOut) return true;
        
        Color targetColor = alphaColor.MultiplyRGB(lightColor).MultiplyRGB(Color.Gray);
        Main.GetItemDrawFrame(item.type, out Texture2D itemTexture, out Rectangle itemFrame);
        Vector2 drawOrigin = itemFrame.Size() / 2f;
        Vector2 drawPosition = item.Bottom - Main.screenPosition - new Vector2(0, drawOrigin.Y);
        
        scale *= Main.essScale;
        //lightColor = lightColor * Main.essScale;
        spriteBatch.Draw(itemTexture, drawPosition, itemFrame, targetColor, rotation, drawOrigin, scale, SpriteEffects.None, 0);
        //spriteBatch.Draw(itemTexture, item.position - Main.screenPosition, itemFrame, targetColor, rotation, itemFrame.Center() * scale, scale, SpriteEffects.None, 0f);
        return false;
    }

    public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor,
        Color itemColor, Vector2 origin, float scale)
    {
        if (!AscendantBurnOut) return true;
        Main.GetItemDrawFrame(item.type, out Texture2D tex, out Rectangle rect);
        spriteBatch.Draw(tex, position, rect, drawColor.MultiplyRGB(Color.Gray), 0f, origin, scale, SpriteEffects.None, 0f);
        return false;
    }

    public override void HoldItem(Item item, Player player)
    {
        if (item.prefix == 0) return;

        if (item.prefix == ModContent.PrefixType<PrefixControlled>()) CurrentBurstCooldown--;

        if (item.prefix == ModContent.PrefixType<PrefixVampiric>())
            player.GetModPlayer<StatPlayer>().Lifesteal += PrefixBalance.VAMPIRIC_LIFESTEAL;

        if (item.prefix == ModContent.PrefixType<PrefixGiantSlayer>())
        {
            player.GetModPlayer<StatPlayer>().TrueDamagePercentage += PrefixBalance.GIANT_SLAYER_PERCENT_DAMAGE;
        }
    }
}