using System.Collections.Generic;
using System.IO;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Ascendant;

public class AscendantGlobalItem : GlobalItem
{
    public override bool InstancePerEntity => true;
    public float DamageDone { get; set; }
    public float DamageDoneRequired { get; private set; }
    public float DamageAdded { get; private set; }
    public float CritAdded { get; private set; }
    public bool AscendantBurnOut { get; private set; }
    
    public override void SaveData(Item item, TagCompound tag)
    {
        if (DamageDone != 0) tag.Add(new KeyValuePair<string, object>(nameof(DamageDone), DamageDone));

        if (DamageDoneRequired != 0) tag.Add(new KeyValuePair<string, object>(nameof(DamageDoneRequired), DamageDoneRequired));
    }

    public override void LoadData(Item item, TagCompound tag)
    {
        DamageDone = tag.TryGet(nameof(DamageDone), out float damage) ? damage : 0;
        DamageDoneRequired = tag.TryGet(nameof(DamageDoneRequired), out float damageDoneRequired) ? damageDoneRequired : 0;

        CheckAscendantBurnOut(item);
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

        CheckAscendantBurnOut(item);
    }
    
    public override bool CanStack(Item destination, Item source)
    {
        int ascendantPrefix = ModContent.PrefixType<PrefixAscendant>();
        return destination.prefix != ascendantPrefix && source.prefix != ascendantPrefix;
    }

    public override bool CanUseItem(Item item, Player player)
    {
        if (item.HasPrefix(ModContent.PrefixType<PrefixAscendant>())) return !AscendantBurnOut;

        return true;
    }

    public override void PostReforge(Item item)
    {
        DamageDone = 0;
        AscendantBurnOut = false;

        DamageDoneRequired = item.HasPrefix(ModContent.PrefixType<PrefixAscendant>()) ? PrefixBalance.GetInfernalDamageRequired(item) : 0;
    }

    public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
    {
        if (item.HasPrefix(ModContent.PrefixType<PrefixAscendant>()))
        {
            float multiplier = MathHelper.Clamp(DamageDone / DamageDoneRequired, 0f, 1f);
            float damageAdded = multiplier * PrefixBalance.INFERNAL_RANGED_MAX_DAMAGE;
            damage.Base += damageAdded * item.damage;
            DamageAdded = damageAdded;

            CheckAscendantBurnOut(item);
        }
    }

    public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
    {
        if (item.HasPrefix(ModContent.PrefixType<PrefixAscendant>()))
        {
            float multiplier = MathHelper.Clamp(DamageDone / DamageDoneRequired, 0f, 1f);
            float critAdded = multiplier * PrefixBalance.INFERNAL_RANGED_MAX_CRIT;
            crit += critAdded;
            CritAdded = critAdded;
        }
    }

    private void CheckAscendantBurnOut(Item item)
    {
        if (item.HasPrefix(ModContent.PrefixType<PrefixAscendant>()) && DamageDone >= DamageDoneRequired && DamageDoneRequired > 0) AscendantBurnOut = true;
    }
    
    public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        if (!item.HasPrefix(ModContent.PrefixType<PrefixAscendant>()) || !AscendantBurnOut) return true;
        
        Color targetColor = alphaColor.MultiplyRGB(lightColor).MultiplyRGB(Color.Gray);
        Main.GetItemDrawFrame(item.type, out Texture2D itemTexture, out Rectangle itemFrame);
        Vector2 drawOrigin = itemFrame.Size() / 2f;
        Vector2 drawPosition = item.Bottom - Main.screenPosition - new Vector2(0, drawOrigin.Y);

        scale *= Main.essScale;
        spriteBatch.Draw(itemTexture, drawPosition, itemFrame, targetColor, rotation, drawOrigin, scale, SpriteEffects.None, 0);
        return false;

    }

    public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        if (!item.HasPrefix(ModContent.PrefixType<PrefixAscendant>()) || !AscendantBurnOut) return true;
        
        Main.GetItemDrawFrame(item.type, out Texture2D tex, out Rectangle rect);
        spriteBatch.Draw(tex, position, rect, drawColor.MultiplyRGB(Color.Gray), 0f, origin, scale, SpriteEffects.None, 0f);
        return false;
    }
}