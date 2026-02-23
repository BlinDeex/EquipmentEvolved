using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EquipmentEvolved.Assets.CharmsModule.Augmentations;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.PrefixDust;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
// Added for TextureAssets

namespace EquipmentEvolved.Assets.CharmsModule.Items;

public class Charm : ModItem
{
    public CharmType CharmType { get; set; } = CharmType.NotInitialized;
    public CharmRarity CharmRarity { get; set; } = CharmRarity.NotInitialized;

    private int charmNameID;
    public int CharmNameID
    {
        get => charmNameID;
        set
        {
            charmNameID = value;
            Item.SetNameOverride(CharmBalance.SplitCamelCase((CharmName)value));
        }
    }

    public string CharmName => CharmBalance.SplitCamelCase((CharmName)CharmNameID);
    public Color CharmColor => CharmBalance.GetCharmColor(CharmRarity);

    public static readonly Dictionary<CharmStat, LocalizedText> StatTexts = new();

    public List<CharmRoll> Stats { get; set; } = [];

    public AugmentationBase Augmentation { get; set; }

    private static Texture2D circleTex = CharmBalance.CharmTextures[CharmType.Circle];
    private static Texture2D squareTex = CharmBalance.CharmTextures[CharmType.Square];
    private static Texture2D triangleTex = CharmBalance.CharmTextures[CharmType.Triangle];
    private static Texture2D noTex = CharmBalance.CharmTextures[CharmType.NotInitialized];

    public override string Texture => $"{Mod.Name}/Assets/CharmsModule/Textures/Charm_NoTex";
    
    private int owner;
    private int ownerPickUpOnlyTicks;
    
    public override void SetDefaults()
    {
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = 1;
        Item.value = 0;
    }
    
    public override void SetStaticDefaults()
    {
        ItemID.Sets.ItemIconPulse[Item.type] = false;
        ItemID.Sets.ItemNoGravity[Item.type] = true;
        
        StatTexts.Add(CharmStat.NotInitialized, LocalizationManager.GetCharmText(CharmStat.NotInitialized));
        StatTexts.Add(CharmStat.Damage, LocalizationManager.GetCharmText(CharmStat.Damage));
        StatTexts.Add(CharmStat.MeleeDamage, LocalizationManager.GetCharmText(CharmStat.MeleeDamage));
        StatTexts.Add(CharmStat.RangedDamage, LocalizationManager.GetCharmText(CharmStat.RangedDamage));
        StatTexts.Add(CharmStat.MagicDamage, LocalizationManager.GetCharmText(CharmStat.MagicDamage));
        StatTexts.Add(CharmStat.SummonDamage, LocalizationManager.GetCharmText(CharmStat.SummonDamage));
        StatTexts.Add(CharmStat.MoveSpeed, LocalizationManager.GetCharmText(CharmStat.MoveSpeed));
        StatTexts.Add(CharmStat.WingTime, LocalizationManager.GetCharmText(CharmStat.WingTime));
        StatTexts.Add(CharmStat.UseSpeed, LocalizationManager.GetCharmText(CharmStat.UseSpeed));
        StatTexts.Add(CharmStat.LifeSteal, LocalizationManager.GetCharmText(CharmStat.LifeSteal));
        StatTexts.Add(CharmStat.CharmLuck, LocalizationManager.GetCharmText(CharmStat.CharmLuck));
        StatTexts.Add(CharmStat.CritDamage, LocalizationManager.GetCharmText(CharmStat.CritDamage));
        StatTexts.Add(CharmStat.ManaUsage, LocalizationManager.GetCharmText(CharmStat.ManaUsage));
        StatTexts.Add(CharmStat.HealingMul, LocalizationManager.GetCharmText(CharmStat.HealingMul));
        StatTexts.Add(CharmStat.Regen, LocalizationManager.GetCharmText(CharmStat.Regen));
        StatTexts.Add(CharmStat.PickSpeed, LocalizationManager.GetCharmText(CharmStat.PickSpeed));
        StatTexts.Add(CharmStat.MaxHealthMul, LocalizationManager.GetCharmText(CharmStat.MaxHealthMul));
        StatTexts.Add(CharmStat.Crit, LocalizationManager.GetCharmText(CharmStat.Crit));
        StatTexts.Add(CharmStat.Iframes, LocalizationManager.GetCharmText(CharmStat.Iframes));
        StatTexts.Add(CharmStat.TrueDamageMul, LocalizationManager.GetCharmText(CharmStat.TrueDamageMul));
        StatTexts.Add(CharmStat.DamageReduction, LocalizationManager.GetCharmText(CharmStat.DamageReduction));
    }
    private Texture2D GetShapeTexture()
    {
        return CharmType switch
        {
            CharmType.Circle => circleTex,
            CharmType.Square => squareTex,
            CharmType.Triangle => triangleTex,
            _ => noTex
        };
    }

    public override void PostUpdate()
    {
        ownerPickUpOnlyTicks--;
        Lighting.AddLight(Item.Center, CharmColor.ToVector3() * 1.0f);

        if (CharmRarity < CharmRarity.Common) return;
        if (!Main.rand.NextBool(3)) return;
        
        Vector2 velocity = -Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * Main.rand.NextFloat(0.5f, 1.5f);
                
        Dust d = Dust.NewDustPerfect(
            Item.Center, 
            ModContent.DustType<CharmDust>(), 
            velocity, 
            0, 
            CharmColor, 
            Main.rand.NextFloat(1.0f, 1.4f) // Slightly larger start scale
        );
                
        d.noLight = true; 
        d.noGravity = true;
    }
    
    public static void DrawCharmShaderStatic(SpriteBatch spriteBatch, Vector2 position, float rotation, float scale, Matrix zoomMatrix, CharmType type, CharmRarity rarity)
    {
        Effect shader = ModContent.Request<Effect>("EquipmentEvolved/Assets/Effects/Charms/CharmShader", AssetRequestMode.ImmediateLoad).Value;
        if (shader == null) return;
        
        float shapeId = type switch
        {
            CharmType.Circle => 1f,
            CharmType.Square => 2f,
            CharmType.Triangle => 3f,
            _ => 1f
        };
        
        Color charmColor = CharmBalance.GetCharmColor(rarity);

        float randomOffset = (position.X + position.Y) * 0.05f;
        
        shader.Parameters["uColor"].SetValue(charmColor.ToVector3());
        shader.Parameters["uOpacity"].SetValue(1f);
        shader.Parameters["uShapeId"].SetValue(shapeId);
        shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
        shader.Parameters["uRarity"].SetValue((float)rarity);
        shader.Parameters["uOffset"].SetValue(randomOffset);
        
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, zoomMatrix);

        shader.CurrentTechnique.Passes[0].Apply();

        Texture2D blankTex = TextureAssets.MagicPixel.Value;
        
        // Ensure scale is consistent
        Vector2 canvasSize = new Vector2(32f, 32f) * scale;
        Vector2 origin = blankTex.Size() / 2f;
        Vector2 scaleVec = canvasSize / blankTex.Size();

        spriteBatch.Draw(blankTex, position, null, Color.White, rotation, origin, scaleVec, SpriteEffects.None, 0f);

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, zoomMatrix);
    }
    
    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        Vector2 position = Item.Center - Main.screenPosition;
        DrawCharmShaderStatic(spriteBatch, position, rotation, scale, Main.GameViewMatrix.TransformationMatrix, CharmType, CharmRarity);
        return false;
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        DrawCharmShaderStatic(spriteBatch, position, 0f, scale, Main.UIScaleMatrix, CharmType, CharmRarity);
        return false;
    }
    
    public void SetOwner(int whoAmI)
    {
        owner = whoAmI;
        ownerPickUpOnlyTicks = 60 * 60;
    }

    public override bool CanPickup(Player player)
    {
        if (ownerPickUpOnlyTicks <= 0) return base.CanPickup(player);
        return player.whoAmI == owner;
    }

    private const string statTypesTag = "statTypes";
    private const string strengthsTag = "strengths";

    private void SaveStats(TagCompound tag)
    {
        if (Stats.Count == 0) return;
        List<int> statTypes = Stats.Select(x => (int)x.Stat).ToList();
        List<float> strengths = Stats.Select(x => x.RawStrength).ToList();
        tag.Add(statTypesTag, statTypes);
        tag.Add(strengthsTag, strengths);
        if(CharmNameID != 0) tag.Add(nameof(CharmNameID), CharmNameID);
        if(Augmentation != null) tag.Add(nameof(Augmentation), Augmentation.GetType().FullName);
    }

    public override void LoadData(TagCompound tag)
    {
        try
        {
            CharmType = tag.ContainsKey(nameof(CharmType)) ? (CharmType)tag.GetInt(nameof(CharmType)) : CharmType.NotInitialized;
            CharmRarity = tag.ContainsKey(nameof(CharmRarity)) ? (CharmRarity)tag.GetInt(nameof(CharmRarity)) : CharmRarity.NotInitialized;
            CharmNameID = tag.ContainsKey(nameof(CharmNameID)) ? tag.GetInt(nameof(CharmNameID)) : 0;
            LoadAugmentation(tag);
            LoadStats(tag);
        }
        catch (Exception e) { Mod.Logger.Error(e); }
    }

    private void LoadAugmentation(TagCompound tag)
    {
        if (!tag.ContainsKey(nameof(Augmentation))) return;
        string typeName = tag.GetString(nameof(Augmentation));
        if (!string.IsNullOrEmpty(typeName))
        {
            try
            {
                Type type = System.Type.GetType(typeName);
                if (type != null && typeof(AugmentationBase).IsAssignableFrom(type))
                    Augmentation = (AugmentationBase)Activator.CreateInstance(type);
            }
            catch (Exception ex) { Mod.Logger.Error(ex.Message); }
        }
    }

    private void LoadStats(TagCompound tag)
    {
        if (!tag.ContainsKey(statTypesTag) || !tag.ContainsKey(strengthsTag)) return;
        List<CharmStat> statTypes = tag.GetList<int>(statTypesTag).Select(x => (CharmStat)x).ToList();
        List<float> strengths = tag.Get<List<float>>(strengthsTag);
        Stats.Clear();
        for (int i = 0; i < statTypes.Count; i++) Stats.Add(new CharmRoll(statTypes[i], strengths[i]));
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips[0].Text = CharmName;
        tooltips[0].OverrideColor = CharmColor;

        TooltipLine rarity = new TooltipLine(Mod, "charmRarity", CharmRarity.ToString())
        {
            OverrideColor = CharmColor
        };
        tooltips.Add(rarity);

        string typeTranslation = CharmType switch
        {
            CharmType.NotInitialized => "Not Initialized",
            CharmType.Circle => " [Weapon]",
            CharmType.Square => " [Armor]",
            CharmType.Triangle => " [Accessory]",
            _ => throw new ArgumentOutOfRangeException()
        };
        
        tooltips.Add(new TooltipLine(Mod, "charmType", CharmType + typeTranslation));

        foreach (CharmRoll roll in Stats)
        {
            float str = roll.GetStrength(CharmBalance.StatDisplayValue(roll.Stat));
            float perfection = CharmBalance.GetRollQualityPercentage(CharmRarity, roll.Stat, roll.RawStrength);
            string text = StatTexts[roll.Stat].Format(str) + $" [{perfection}%]";
            tooltips.Add(new TooltipLine(Mod, "statLine", text) { OverrideColor = CharmBalance.GetStatColor(roll.Stat) });
        }

        if (Augmentation != null)
        {
            tooltips.Add(new TooltipLine(Mod, "augmentation", Augmentation.LocalizedTooltip)
            {
                OverrideColor = Color.Lerp(Main.DiscoColor, Color.LightGray, 0.4f)
            });
        }
    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.Write((sbyte)CharmRarity);
        writer.Write((byte)CharmType);
        writer.Write((byte)CharmNameID);
        bool hasAugmentation = Augmentation != null;
        writer.Write(hasAugmentation);
        if(hasAugmentation) writer.Write(Augmentation.GetType().FullName!);
        writer.Write((byte)Stats.Count);
        foreach (CharmRoll roll in Stats)
        {
            writer.Write((byte)roll.Stat);
            writer.Write(roll.RawStrength);
        }
    }

    public override void NetReceive(BinaryReader reader)
    {
        CharmRarity = (CharmRarity)reader.ReadSByte();
        CharmType = (CharmType)reader.ReadByte();
        CharmNameID = reader.ReadByte();
        if (reader.ReadBoolean())
        {
            Type augmentationType = System.Type.GetType(reader.ReadString());
            Augmentation = (AugmentationBase)Activator.CreateInstance(augmentationType!);
        }
        int statsCount = reader.ReadByte();
        Stats.Clear();
        for (int i = 0; i < statsCount; i++)
        {
            Stats.Add(new CharmRoll((CharmStat)reader.ReadByte(), reader.ReadSingle()));
        }
    }
    
    public override void SaveData(TagCompound tag)
    {
        tag.Add(nameof(CharmType),(int)CharmType);
        tag.Add(nameof(CharmRarity),(int)CharmRarity);
        SaveStats(tag);
    }
}