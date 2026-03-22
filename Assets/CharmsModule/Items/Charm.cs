using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.CharmsModule.Augmentations;
using EquipmentEvolved.Assets.CharmsModule.Data;
using EquipmentEvolved.Assets.CharmsModule.Dusts;
using EquipmentEvolved.Assets.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace EquipmentEvolved.Assets.CharmsModule.Items;

public class Charm : ModItem
{
    private int charmNameID;

    private int owner;
    private int ownerPickUpOnlyTicks;
    public CharmType CharmType { get; set; } = CharmType.NotInitialized;
    public CharmRarity CharmRarity { get; set; } = CharmRarity.NotInitialized;
    
    public Guid UniqueID { get; set; } = Guid.NewGuid();

    public int CharmNameID
    {
        get => charmNameID;
        set
        {
            charmNameID = value;
            Item.SetNameOverride(CharmName + " Charm"); 
        }
    }

    public string CharmName 
    {
        get
        {
            if (CharmNameID == 0) return "Dormant"; 
            return CharmBalance.SplitCamelCase((CharmName)CharmNameID);
        }
    }
    public Color CharmColor => CharmBalance.GetCharmColor(CharmRarity);

    public List<CharmRoll> Stats { get; set; } = [];

    public List<AugmentationBase> Augmentations { get; set; } = [];

    public override string Texture => $"{Mod.Name}/Assets/CharmsModule/Textures/Charm_NoTex";

    public override void SetDefaults()
    {
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = 99; // magic storage fix
        Item.value = 0;
    }

    public override void SetStaticDefaults()
    {
        ItemID.Sets.ItemIconPulse[Item.type] = false;
        ItemID.Sets.ItemNoGravity[Item.type] = true;
    }
    
    public override ModItem Clone(Item newEntity)
    {
        Charm clone = (Charm)base.Clone(newEntity);

        if (Stats != null) clone.Stats = [..Stats];
        if (Augmentations != null) clone.Augmentations = [..Augmentations];
        clone.UniqueID = UniqueID;
        if (CharmNameID != 0)
        {
            newEntity.SetNameOverride(CharmName + " Charm");
        }

        return clone;
    }
    
    public override bool CanStack(Item source)
    {
        return false;
    }

    public override void PostUpdate()
    {
        ownerPickUpOnlyTicks--;
        Lighting.AddLight(Item.Center, CharmColor.ToVector3() * 1.0f);

        if (CharmRarity < CharmRarity.Common) return;

        if (!Main.rand.NextBool(3)) return;

        Vector2 velocity = -Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * Main.rand.NextFloat(0.5f, 1.5f);

        Dust d = Dust.NewDustPerfect(Item.Center, ModContent.DustType<CharmDust>(), velocity, 0, CharmColor, Main.rand.NextFloat(1.0f, 1.4f));

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

        shader.Parameters["uColor"].SetValue(rarity == CharmRarity.Exalted ? Main.DiscoColor.ToVector3() * 2 : charmColor.ToVector3());
        shader.Parameters["uOpacity"].SetValue(1f);
        shader.Parameters["uShapeId"].SetValue(shapeId);
        shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
        shader.Parameters["uRarity"].SetValue((float)rarity);
        shader.Parameters["uOffset"].SetValue(randomOffset);

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, zoomMatrix);

        shader.CurrentTechnique.Passes[0].Apply();

        Texture2D blankTex = TextureAssets.MagicPixel.Value;
        
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

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        TooltipLine nameLine = tooltips.FirstOrDefault(t => t.Name == "ItemName" && t.Mod == "Terraria");

        if (nameLine != null)
        {
            // 2. DORMANT FIX: If they somehow get the base item, hide the blank stats
            if (CharmNameID == 0)
            {
                nameLine.Text = "Dormant Charm";
                nameLine.OverrideColor = Color.Gray;
                
                tooltips.Add(new TooltipLine(Mod, "DormantDesc", "A dormant, unformed charm.\nUsed for crafting."));
                return; // STOP HERE! Don't load blank stats.
            }

            // 3. NORMAL CHARMS: Apply the correct rolled name and color!
            nameLine.Text = CharmName + " Charm";
            nameLine.OverrideColor = CharmColor;
        }
        
        tooltips[0].Text = CharmName;
        tooltips[0].OverrideColor = CharmColor;
        
        foreach (CharmRoll roll in Stats)
        {
            float perfection = CharmBalance.GetRollQualityPercentage(CharmRarity, roll.Stat, roll.Strength);
            string text = roll.GetTooltip() + $" [{perfection}%]";
    
            tooltips.Add(new TooltipLine(Mod, "statLine", text) { OverrideColor = CharmBalance.GetStatColor(roll.Stat) });
        }

        if (Augmentations == null || Augmentations.Count <= 0) return;

        for (int i = 0; i < Augmentations.Count; i++)
        {
            tooltips.Add(new TooltipLine(Mod, $"augmentation_{i}", Augmentations[i].LocalizedTooltip)
            {
                OverrideColor = Color.Lerp(Main.DiscoColor, Color.LightGray, 0.4f)
            });
        }
    }
    
    public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
    {
        if (line.Name == "ItemName" && line.Mod == "Terraria")
        {
            Effect charmShader = ModContent.Request<Effect>("EquipmentEvolved/Assets/Effects/Charms/CharmTextShader", AssetRequestMode.ImmediateLoad).Value;
            if (charmShader == null) return true;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
            charmShader.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly);
            charmShader.CurrentTechnique.Passes[0].Apply();
        }

        return true; 
    }
    
    public override void PostDrawTooltipLine(DrawableTooltipLine line)
    {
        if (line.Name == "ItemName" && line.Mod == "Terraria")
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
        }
    }
    
    public override void SaveData(TagCompound tag)
    {
        tag.Add(nameof(UniqueID), UniqueID.ToString());
        tag.Add(nameof(CharmType), (int)CharmType);
        tag.Add(nameof(CharmRarity), (int)CharmRarity);

        if (CharmNameID != 0) tag.Add(nameof(CharmNameID), CharmNameID);
        if (Stats.Count > 0)
        {
            List<TagCompound> statTags = Stats.Select(x => x.SaveData()).ToList();
            tag.Add("StatsList", statTags);
        }

        if (Augmentations == null || Augmentations.Count <= 0) return;

        List<string> augTypeNames = Augmentations.Select(x => x.GetType().FullName).ToList();
        tag.Add(nameof(Augmentations), augTypeNames);
    }

    public override void LoadData(TagCompound tag)
    {
        try
        {
            CharmType = tag.ContainsKey(nameof(CharmType)) ? (CharmType)tag.GetInt(nameof(CharmType)) : CharmType.NotInitialized;
            CharmRarity = tag.ContainsKey(nameof(CharmRarity)) ? (CharmRarity)tag.GetInt(nameof(CharmRarity)) : CharmRarity.NotInitialized;
            CharmNameID = tag.ContainsKey(nameof(CharmNameID)) ? tag.GetInt(nameof(CharmNameID)) : 0;
            UniqueID = tag.ContainsKey(nameof(UniqueID)) ? Guid.Parse(tag.GetString(nameof(UniqueID))) : Guid.NewGuid();
        
            Stats.Clear();
            if (tag.ContainsKey("StatsList"))
            {
                IList<TagCompound> statTags = tag.GetList<TagCompound>("StatsList");
                foreach (TagCompound statTag in statTags)
                {
                    CharmRoll roll = CharmRoll.LoadData(statTag);
                    if (roll != null) Stats.Add(roll);
                }
            }

            Augmentations.Clear();
            if (!tag.ContainsKey(nameof(Augmentations))) return;

            IList<string> typeNames = tag.GetList<string>(nameof(Augmentations));

            foreach (string typeName in typeNames)
            {
                if (string.IsNullOrEmpty(typeName)) continue;

                try
                {
                    Type type = System.Type.GetType(typeName);
                    if (type != null && typeof(AugmentationBase).IsAssignableFrom(type)) Augmentations.Add((AugmentationBase)Activator.CreateInstance(type));
                }
                catch (Exception ex)
                {
                    Mod.Logger.Error($"Failed to load augmentation '{typeName}': {ex.Message}");
                }
            }
        }
        catch (Exception e)
        {
            Mod.Logger.Error(e);
        }
    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.Write(UniqueID.ToByteArray());

        writer.Write((sbyte)CharmRarity);
        writer.Write((byte)CharmType);
        writer.Write((byte)CharmNameID);

        writer.Write((byte)Augmentations.Count);
        foreach (AugmentationBase aug in Augmentations)
        {
            writer.Write(aug.GetType().FullName!);
        }
        writer.Write((byte)Stats.Count);
        foreach (CharmRoll roll in Stats)
        {
            roll.NetSend(writer);
        }
    }

    public override void NetReceive(BinaryReader reader)
    {
        UniqueID = new Guid(reader.ReadBytes(16));

        CharmRarity = (CharmRarity)reader.ReadSByte();
        CharmType = (CharmType)reader.ReadByte();
        CharmNameID = reader.ReadByte();

        int augCount = reader.ReadByte();
        Augmentations.Clear();
        for (int i = 0; i < augCount; i++)
        {
            Type augmentationType = System.Type.GetType(reader.ReadString());
            if (augmentationType != null) Augmentations.Add((AugmentationBase)Activator.CreateInstance(augmentationType));
        }
        
        int statsCount = reader.ReadByte();
        Stats.Clear();
        for (int i = 0; i < statsCount; i++)
        {
            CharmRoll roll = CharmRoll.NetReceive(reader);
            if (roll != null) Stats.Add(roll);
        }
    }
}