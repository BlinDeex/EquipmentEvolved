using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.CharmsModule.Augmentations;
using EquipmentEvolved.Assets.CharmsModule.Data;
using EquipmentEvolved.Assets.CharmsModule.Dusts;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace EquipmentEvolved.Assets.CharmsModule.Items;

public class Charm : ModItem
{
    private const string statTypesTag = "statTypes";
    private const string strengthsTag = "strengths";

    public static readonly Dictionary<PlayerStat, LocalizedText> StatTexts = new();

    private int charmNameID;

    private int owner;
    private int ownerPickUpOnlyTicks;
    public CharmType CharmType { get; set; } = CharmType.NotInitialized;
    public CharmRarity CharmRarity { get; set; } = CharmRarity.NotInitialized;

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

    public List<CharmRoll> Stats { get; set; } = [];

    public List<AugmentationBase> Augmentations { get; set; } = [];

    public override string Texture => $"{Mod.Name}/Assets/CharmsModule/Textures/Charm_NoTex";

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

        StatTexts.Add(PlayerStat.NotInitialized, LocalizationManager.GetCharmText(PlayerStat.NotInitialized));

        foreach (PlayerStat stat in CharmBalance.ValidCharmStats)
        {
            if (!StatTexts.ContainsKey(stat)) StatTexts.Add(stat, LocalizationManager.GetCharmText(stat));
        }
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

    public override void SaveData(TagCompound tag)
    {
        tag.Add(nameof(CharmType), (int)CharmType);
        tag.Add(nameof(CharmRarity), (int)CharmRarity);

        if (CharmNameID != 0) tag.Add(nameof(CharmNameID), CharmNameID);

        if (Stats.Count > 0)
        {
            List<int> statTypes = Stats.Select(x => (int)x.Stat).ToList();
            List<float> strengths = Stats.Select(x => x.RawStrength).ToList();
            tag.Add(statTypesTag, statTypes);
            tag.Add(strengthsTag, strengths);
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
            
            Stats.Clear();
            if (tag.ContainsKey(statTypesTag) && tag.ContainsKey(strengthsTag))
            {
                List<PlayerStat> statTypes = tag.GetList<int>(statTypesTag).Select(x => (PlayerStat)x).ToList();
                List<float> strengths = tag.GetList<float>(strengthsTag).ToList();

                for (int i = 0; i < statTypes.Count; i++)
                {
                    Stats.Add(new CharmRoll(statTypes[i], strengths[i]));
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

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips[0].Text = CharmName;
        tooltips[0].OverrideColor = CharmColor;

        TooltipLine rarity = new(Mod, "charmRarity", CharmRarity.ToString())
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
            float str = roll.GetStrength();
            float perfection = CharmBalance.GetRollQualityPercentage(CharmRarity, roll.Stat, roll.RawStrength);
            string text = StatTexts[roll.Stat].Format(str) + $" [{perfection}%]";
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

    public override void NetSend(BinaryWriter writer)
    {
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
            writer.Write((byte)roll.Stat);
            writer.Write(roll.RawStrength);
        }
    }

    public override void NetReceive(BinaryReader reader)
    {
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
            Stats.Add(new CharmRoll((PlayerStat)reader.ReadByte(), reader.ReadSingle()));
        }
    }
}