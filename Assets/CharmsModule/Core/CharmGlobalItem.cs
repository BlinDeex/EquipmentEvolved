using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.CharmsModule.Augmentations;
using EquipmentEvolved.Assets.CharmsModule.Data;
using EquipmentEvolved.Assets.CharmsModule.Items;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.Symbiotic;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;

namespace EquipmentEvolved.Assets.CharmsModule.Core;

public class CharmGlobalItem : GlobalItem
{
    private const string statTypesTag = "statTypesItem";
    private const string strengthsTag = "strengthsItem";

    private float _maxCharmLineWidth;
    private float _totalCharmBlockHeight;

    private List<CharmRoll> appliedStats = [];

    private List<AugmentationBase> augmentations = [];
    public CharmDataSnapshot BaseSnapshot;
    private string charmName;

    public bool holdingItem;

    public bool IsSymbioticUpgraded;
    public override bool InstancePerEntity => true;
    public CharmRarity AppliedCharmRarity { get; private set; } = CharmRarity.NotInitialized;

    private bool hasCharm => appliedStats.Count > 0;

    public CharmDataSnapshot GetCurrentData()
    {
        return new CharmDataSnapshot
        {
            Rarity = AppliedCharmRarity,
            Augmentations = augmentations,
            Stats = [..appliedStats],
            CharmName = charmName
        };
    }

    public void LoadFromData(CharmDataSnapshot data)
    {
        AppliedCharmRarity = data.Rarity;
        augmentations = data.Augmentations;
        appliedStats = new List<CharmRoll>(data.Stats);
        charmName = data.CharmName;
    }

    public override void UpdateEquip(Item item, Player player)
    {
        ApplyHeldStats(player);
    }
    
    public void ApplyCharm(Charm charm, Item targetItem)
    {
        AppliedCharmRarity = charm.CharmRarity;
        augmentations = new List<AugmentationBase>(charm.Augmentations);
        charmName = charm.CharmName;
        appliedStats = new List<CharmRoll>(charm.Stats);

        if (targetItem.HasPrefix(ModContent.PrefixType<PrefixSymbiotic>()))
        {
            for (int i = 0; i < appliedStats.Count; i++)
            {
                CharmRoll roll = appliedStats[i];

                float currentQuality = CharmBalance.GetRollQuality(roll.Stat, AppliedCharmRarity, roll.Strength);
                float newQuality = Math.Min(currentQuality + CharmBalance.SYMBIOTIC_BASE_QUALITY_BOOST, 1f);

                float newStrength = CharmBalance.CalculateStrengthFromQuality(roll.Stat, AppliedCharmRarity, newQuality);
                appliedStats[i] = new CharmRoll(roll.Stat, newStrength);
            }
        }

        Main.mouseItem.TurnToAir();
        Color textColor = CharmBalance.GetCharmColor(charm.CharmRarity);
        Main.NewText($"Applied {charm.CharmRarity} charm to {targetItem.Name}!", textColor);
    }

    public static bool CanApplyCharm(Player player, Item targetItem)
    {
        Item mouseItem = player.inventory[58];
        bool isCharm = mouseItem.type == ModContent.ItemType<Charm>();

        if (!isCharm) return false;

        Charm charmItem = (Charm)mouseItem.ModItem;

        bool weapon = targetItem.IsWeapon();
        bool armor = targetItem.IsArmor();
        bool accessory = targetItem.accessory && !armor && !weapon;

        CharmType type = charmItem.CharmType;

        return type switch
        {
            CharmType.NotInitialized => false,
            CharmType.Circle => weapon,
            CharmType.Square => armor,
            CharmType.Triangle => accessory,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void ApplyHeldStats(Player player)
    {
        if (appliedStats.Count > 0)
        {
            player.GetModPlayer<StatPlayer>().ApplyCharmStats(appliedStats);
        }

        if (augmentations != null && augmentations.Count > 0)
        {
            foreach (AugmentationBase augmentationBase in augmentations)
            {
                augmentationBase.EnableAugmentation(player);
            }
        }
    }

    public override void HoldItem(Item item, Player player)
    {
        if (!item.IsWeapon()) return;

        // Stats and Augmentations are applied in StatPlayer.PostUpdateEquips
        // this hook is just to toggle the tooltip warning logic.

        if (appliedStats.Count > 0) holdingItem = true;
    }

    public override void UpdateInventory(Item item, Player player)
    {
        holdingItem = false;
    }

    public override void SaveData(Item item, TagCompound tag)
    {
        CharmDataSnapshot dataToSave = IsSymbioticUpgraded && BaseSnapshot != null ? BaseSnapshot : GetCurrentData();

        if (dataToSave.Augmentations != null && dataToSave.Augmentations.Count > 0)
        {
            List<string> augTypeNames = dataToSave.Augmentations.Select(x => x.GetType().FullName).ToList();
            tag.Add("augmentations", augTypeNames);
        }

        if (dataToSave.CharmName != string.Empty) tag.Add("charmName", dataToSave.CharmName);

        SaveStats(tag, dataToSave);
    }

    private void SaveStats(TagCompound tag, CharmDataSnapshot data)
    {
        if (data.Stats == null || data.Stats.Count == 0) return;

        tag.Add(nameof(AppliedCharmRarity), (int)data.Rarity);
        List<TagCompound> statTags = data.Stats.Select(x => x.SaveData()).ToList();
        tag.Add("StatsList", statTags);
    }

    private void SaveStats(TagCompound tag)
    {
        if (appliedStats.Count == 0) return;

        tag.Add(nameof(AppliedCharmRarity), (int)AppliedCharmRarity);
        List<TagCompound> statTags = appliedStats.Select(x => x.SaveData()).ToList();
        tag.Add("StatsList", statTags);
    }

    public override void LoadData(Item item, TagCompound tag)
    {
        try
        {
            charmName = tag.ContainsKey("charmName") ? tag.GetString("charmName") : string.Empty;
            LoadAugmentations(tag);
            LoadStats(tag);
        }
        catch (Exception e)
        {
            Mod.Logger.Error(e.Message);
        }
    }

    private void LoadStats(TagCompound tag)
    {
        appliedStats.Clear(); 

        charmName = tag.ContainsKey("charmName") ? tag.GetString("charmName") : string.Empty;
        AppliedCharmRarity = tag.ContainsKey(nameof(AppliedCharmRarity)) ? (CharmRarity)tag.GetInt(nameof(AppliedCharmRarity)) : CharmRarity.NotInitialized;
        if (tag.ContainsKey("StatsList"))
        {
            IList<TagCompound> statTags = tag.GetList<TagCompound>("StatsList");
            foreach (TagCompound statTag in statTags)
            {
                CharmRoll roll = CharmRoll.LoadData(statTag);
                if (roll != null) appliedStats.Add(roll);
            }
        }
    }

    private void LoadAugmentations(TagCompound tag)
    {
        augmentations.Clear();
        if (!tag.ContainsKey(nameof(augmentations))) return;

        IList<string> typeNames = tag.GetList<string>(nameof(augmentations));

        foreach (string typeName in typeNames)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                Mod.Logger.Error("Saved augmentation type name is null or empty.");
                continue;
            }

            try
            {
                Type type = Type.GetType(typeName);
                if (type == null || !typeof(AugmentationBase).IsAssignableFrom(type))
                {
                    Mod.Logger.Error($"Type '{typeName}' is not a valid subclass of AugmentationBase or could not be found.");
                    continue;
                }

                if (Activator.CreateInstance(type) is not AugmentationBase augmentationBase)
                {
                    Mod.Logger.Error($"Failed to instantiate augmentation type: {typeName}. It may be abstract or lack a valid constructor.");
                    continue;
                }

                augmentations.Add(augmentationBase);
            }
            catch (Exception ex)
            {
                Mod.Logger.Error($"Error occurred while loading augmentation type '{typeName}': {ex.Message}");
            }
        }
    }
    
    public override void NetSend(Item item, BinaryWriter writer)
    {
        CharmDataSnapshot dataToSend = IsSymbioticUpgraded && BaseSnapshot != null ? BaseSnapshot : GetCurrentData();

        bool hasAugmentation = dataToSend.Augmentations != null && dataToSend.Augmentations.Count > 0;
        bool hasStats = dataToSend.Stats.Count > 0;

        bool hasData = hasStats || hasAugmentation;
        writer.Write(hasData);

        if (!hasData) return;

        writer.Write(dataToSend.CharmName);

        byte augCount = (byte)(dataToSend.Augmentations?.Count ?? 0);
        writer.Write(augCount);

        if (dataToSend.Augmentations != null)
        {
            foreach (AugmentationBase aug in dataToSend.Augmentations)
            {
                writer.Write(aug.GetType().FullName!);
            }
        }

        writer.Write((byte)dataToSend.Rarity);
        writer.Write((byte)dataToSend.Stats.Count);
        foreach (CharmRoll roll in dataToSend.Stats)
        {
            roll.NetSend(writer);
        }
    }

    public override void NetReceive(Item item, BinaryReader reader)
    {
        bool hasData = reader.ReadBoolean();
        if (!hasData) return;

        charmName = reader.ReadString();

        int augCount = reader.ReadByte();
        augmentations.Clear();

        for (int i = 0; i < augCount; i++)
        {
            string typeName = reader.ReadString();
            Type augmentationType = Type.GetType(typeName);

            if (augmentationType != null && typeof(AugmentationBase).IsAssignableFrom(augmentationType))
                augmentations.Add((AugmentationBase)Activator.CreateInstance(augmentationType));
            else
                Mod.Logger.Warn($"Failed to resolve Augmentation type: {typeName}");
        }

        AppliedCharmRarity = (CharmRarity)reader.ReadByte();

        // FIXED: Now correctly reads a single byte just like NetSend wrote!
        int count = reader.ReadByte(); 
        
        appliedStats = new List<CharmRoll>(count);
        for (int i = 0; i < count; i++)
        {
            CharmRoll roll = CharmRoll.NetReceive(reader);
            if (roll != null) appliedStats.Add(roll);
        }
    }
    

    public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        if (!hasCharm || AppliedCharmRarity == CharmRarity.NotInitialized) return;

        CharmType drawnType = CharmType.NotInitialized;

        if (item.IsWeapon())
            drawnType = CharmType.Circle;
        else if (item.IsArmor())
            drawnType = CharmType.Square;
        else if (item.accessory) drawnType = CharmType.Triangle;

        if (drawnType == CharmType.NotInitialized) return;

        Vector2 offset = new Vector2(-16f, -16f) * Main.inventoryScale;
        Vector2 drawPos = position + offset;

        float miniScale = 0.5f * Main.inventoryScale;

        Charm.DrawCharmShaderStatic(spriteBatch, drawPos, 0f, miniScale, Main.UIScaleMatrix, drawnType, AppliedCharmRarity);
    }
    

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (appliedStats.Count == 0) return;

        if (item.IsWeapon())
        {
            bool holding = item.GetGlobalItem<CharmGlobalItem>().holdingItem;
            string text = holding ? "Active" : "You must hold this weapon to gain charm stats";

            tooltips.Add(new TooltipLine(Mod, "CharmWeaponTooltipLine", text)
            {
                IsModifier = true,
                IsModifierBad = !holding
            });
        }

        if (!hasCharm) return;

        _maxCharmLineWidth = 0f;
        _totalCharmBlockHeight = 0f;

        string headerText = $"{charmName} [{AppliedCharmRarity}]";
        AddCharmLine(tooltips, "CharmHeader", headerText, CharmBalance.GetCharmColor(AppliedCharmRarity));

        for (int i = 0; i < appliedStats.Count; i++)
        {
            CharmRoll roll = appliedStats[i];
            
            float perfection = CharmBalance.GetRollQualityPercentage(AppliedCharmRarity, roll.Stat, roll.Strength);
            string text = roll.GetTooltip() + $" [{perfection}%]";

            AddCharmLine(tooltips, $"CharmStat_{i}", text, CharmBalance.GetStatColor(roll.Stat));
        }

        if (augmentations != null && augmentations.Count > 0)
        {
            for (int i = 0; i < augmentations.Count; i++)
            {
                AddCharmLine(tooltips, $"CharmAugmentation_{i}", augmentations[i].LocalizedTooltip, Color.Lerp(Main.DiscoColor, Color.LightGray, 0.4f));
            }
        }
    }

    private void AddCharmLine(List<TooltipLine> tooltips, string name, string text, Color color)
    {
        TooltipLine line = new(Mod, name, text)
        {
            OverrideColor = color
        };
        tooltips.Add(line);

        Vector2 size = ChatManager.GetStringSize(FontAssets.MouseText.Value, text, Vector2.One);

        if (size.X > _maxCharmLineWidth) _maxCharmLineWidth = size.X;

        _totalCharmBlockHeight += size.Y;
    }

    public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
    {
        if (line.Mod != Mod.Name) return true;

        if (line.Name == "CharmHeader")
        {
            SpriteBatch sb = Main.spriteBatch;

            int paddingX = 10;
            int liftAmount = 8;
            int extraBottomPad = 0;

            int boxX = line.X - paddingX;
            int boxY = line.Y - liftAmount;

            int boxWidth = (int)_maxCharmLineWidth + paddingX * 2;
            // ReSharper disable once UselessBinaryOperation
            int boxHeight = (int)_totalCharmBlockHeight + liftAmount + extraBottomPad;
            Color borderColor = CharmBalance.GetCharmColor(AppliedCharmRarity);
            Color bgColor = new(0, 0, 0, 150);

            Texture2D pixel = TextureAssets.MagicPixel.Value;

            sb.Draw(pixel, new Rectangle(boxX, boxY, boxWidth, boxHeight), bgColor);

            int borderThick = 2;

            // Top
            sb.Draw(pixel, new Rectangle(boxX, boxY, boxWidth, borderThick), borderColor);
            // Bottom
            sb.Draw(pixel, new Rectangle(boxX, boxY + boxHeight - borderThick, boxWidth, borderThick), borderColor);
            // Left
            sb.Draw(pixel, new Rectangle(boxX, boxY, borderThick, boxHeight), borderColor);
            // Right
            sb.Draw(pixel, new Rectangle(boxX + boxWidth - borderThick, boxY, borderThick, boxHeight), borderColor);
        }

        return true;
    }
}