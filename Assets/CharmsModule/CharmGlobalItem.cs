using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EquipmentEvolved.Assets.CharmsModule.Augmentations;
using EquipmentEvolved.Assets.CharmsModule.Items;
using EquipmentEvolved.Assets.ModPlayers;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;

namespace EquipmentEvolved.Assets.CharmsModule;

public class CharmGlobalItem : GlobalItem
{
    public override bool InstancePerEntity => true;

    private List<CharmRoll> appliedStats = [];
    private CharmRarity appliedCharmRarity = CharmRarity.NotInitialized;
    private string charmName;
    private const string statTypesTag = "statTypesItem";
    private const string strengthsTag = "strengthsItem";

    private bool hasCharm => appliedStats.Count > 0;

    public bool holdingItem;

    private AugmentationBase augmentation;
    
    private float _maxCharmLineWidth = 0f;
    private float _totalCharmBlockHeight = 0f; // Track total height for single-box drawing

    private void ApplyCharm(Charm charm, string targetItemName)
    {
        appliedStats = charm.Stats;
        appliedCharmRarity = charm.CharmRarity;
        augmentation = charm.Augmentation;
        charmName = charm.CharmName;
        Main.mouseItem.TurnToAir();
        Color textColor = CharmBalance.GetCharmColor(charm.CharmRarity);
        
        Main.NewText($"Applied {charm.CharmRarity} charm to {targetItemName}!", textColor);
    }
    
    public override void RightClick(Item item, Player player)
    {
        if (!CanApplyCharm(Main.LocalPlayer, item)) return;
        item.stack++;
        
        ApplyCharm((Charm)player.inventory[58].ModItem, item.Name);
    }

    private bool CanApplyCharm(Player player, Item targetItem)
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

    public override bool CanRightClick(Item item)
    {
        return CanApplyCharm(Main.LocalPlayer, item);
    }

    public override void UpdateEquip(Item item, Player player)
    {
        if (appliedStats.Count == 0) return;
        
        player.GetModPlayer<StatPlayer>().ApplyCharmStats(appliedStats);
        augmentation?.FlagEnabler(player);
    }

    public override void HoldItem(Item item, Player player)
    {
        if (!item.IsWeapon()) return;
        CharmGlobalItem charmItem = item.GetGlobalItem<CharmGlobalItem>();
        augmentation?.FlagEnabler(player);
        
        if (!charmItem.hasCharm) return;
        
        player.GetModPlayer<StatPlayer>().ApplyCharmStats(appliedStats);
        holdingItem = true;
    }

    public override void UpdateInventory(Item item, Player player)
    {
        holdingItem = false;
    }

    public override void SaveData(Item item, TagCompound tag)
    {
        if(augmentation != null) tag.Add(nameof(augmentation), augmentation.GetType().FullName);
        if(charmName != string.Empty) tag.Add(nameof(charmName), charmName);
        
        SaveStats(tag);
    }

    public override void LoadData(Item item, TagCompound tag)
    {
        try
        {
            charmName = tag.ContainsKey(nameof(charmName)) ? tag.GetString(nameof(charmName)) : string.Empty;
            LoadAugmentation(tag);
            LoadStats(tag);
        }
        catch (Exception e)
        {
            Mod.Logger.Error(e.Message);
        }
    }
    
    private void LoadAugmentation(TagCompound tag)
    {
        if (!tag.ContainsKey(nameof(augmentation))) return;
    
        string typeName = tag.GetString(nameof(augmentation));
    
        // Guard clause: Check for null/empty string
        if (string.IsNullOrEmpty(typeName))
        {
            Mod.Logger.Error("Saved augmentation type name is null or empty.");
            return;
        }

        try
        {
            Type type = Type.GetType(typeName);
            
            // Guard clause: Check if type is missing or invalid
            if (type == null || !typeof(AugmentationBase).IsAssignableFrom(type))
            {
                Mod.Logger.Error($"Type '{typeName}' is not a valid subclass of AugmentationBase or could not be found.");
                return;
            }

            // Guard clause: Try to instantiate and check if it failed
            if (Activator.CreateInstance(type) is not AugmentationBase augmentationBase)
            {
                Mod.Logger.Error($"Failed to instantiate augmentation type: {typeName}. It may be abstract or lack a valid constructor.");
                return;
            }

            // Happy path: Everything succeeded
            augmentation = augmentationBase;
        }
        catch (Exception ex)
        {
            Mod.Logger.Error($"Error occurred while loading augmentation type '{typeName}': {ex.Message}");
        }
    }

    public override void NetSend(Item item, BinaryWriter writer)
    {
        bool hasAugmentation = augmentation != null;
        bool hasStats = appliedStats.Count > 0;
        
        // We only need to send data if we actually have stats or an augmentation
        bool hasData = hasStats || hasAugmentation;
        writer.Write(hasData);
        
        if (!hasData) return;
        
        // 1. Send Augmentation Data
        writer.Write(charmName);
        writer.Write(hasAugmentation);
        if (hasAugmentation)
        {
            writer.Write(augmentation.GetType().FullName!);
        }
        
        // 2. Send Rarity
        writer.Write((byte)appliedCharmRarity);
        
        // 3. Send Stats (Count -> Loop -> Type/Strength)
        writer.Write(appliedStats.Count);
        foreach (CharmRoll roll in appliedStats)
        {
            writer.Write((byte)roll.Stat);
            writer.Write(roll.RawStrength);
        }
    }

    public override void NetReceive(Item item, BinaryReader reader)
    {
        // Check if there is data to read
        bool hasData = reader.ReadBoolean();
        if (!hasData) return;

        charmName = reader.ReadString();
        bool hasAugmentation = reader.ReadBoolean();
        if (hasAugmentation)
        {
            string typeName = reader.ReadString();
            Type augmentationType = Type.GetType(typeName);

            if (augmentationType != null && typeof(AugmentationBase).IsAssignableFrom(augmentationType))
            {
                augmentation = (AugmentationBase)Activator.CreateInstance(augmentationType);
            }
            else
            {
                Mod.Logger.Warn($"Failed to resolve Augmentation type: {typeName}");
            }
        }

        // 2. Receive Rarity
        appliedCharmRarity = (CharmRarity)reader.ReadByte();

        // 3. Receive Stats
        int count = reader.ReadInt32();
        appliedStats = new List<CharmRoll>(count);

        for (int i = 0; i < count; i++)
        {
            CharmStat stat = (CharmStat)reader.ReadByte();
            float strength = reader.ReadSingle();
            appliedStats.Add(new CharmRoll(stat, strength));
        }
    }

    private void SaveStats(TagCompound tag)
    {
        if (appliedStats.Count == 0) return;
        
        tag.Add(nameof(appliedCharmRarity),(int)appliedCharmRarity);
        
        List<int> statTypes = appliedStats.Select(x => (int)x.Stat).ToList();
        List<float> strengths = appliedStats.Select(x => x.RawStrength).ToList();
        
        tag.Add(statTypesTag, statTypes);
        tag.Add(strengthsTag, strengths);
    }
    
    public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        if (!hasCharm || appliedCharmRarity == CharmRarity.NotInitialized) return;
        
        CharmType drawnType = CharmType.NotInitialized;
        
        if (item.IsWeapon()) drawnType = CharmType.Circle;
        else if (item.IsArmor()) drawnType = CharmType.Square;
        else if (item.accessory) drawnType = CharmType.Triangle;

        if (drawnType == CharmType.NotInitialized) return;
        
        Vector2 offset = new Vector2(-16f, -16f) * Main.inventoryScale; // Top-Left
        Vector2 drawPos = position + offset;

        // 3. Draw the Mini Charm
        // We use a small scale (e.g., 0.4x) so it looks like a badge
        float miniScale = 0.5f * Main.inventoryScale;

        Charm.DrawCharmShaderStatic(spriteBatch, drawPos, 0f, miniScale, Main.UIScaleMatrix, drawnType, appliedCharmRarity);
    }
    
    private void LoadStats(TagCompound tag)
    {
        if (!tag.ContainsKey(statTypesTag) || !tag.ContainsKey(strengthsTag)) return;
        charmName = tag.ContainsKey(nameof(charmName)) ? tag.GetString(nameof(charmName)) : string.Empty;
        appliedCharmRarity = tag.ContainsKey(nameof(appliedCharmRarity)) ? (CharmRarity)tag.GetInt(nameof(appliedCharmRarity)) : CharmRarity.NotInitialized;
        
        List<CharmStat> statTypes = tag.GetList<int>(statTypesTag).Select(x => (CharmStat)x).ToList();
        List<float> strengths = tag.Get<List<float>>(strengthsTag);

        if (statTypes.Count != strengths.Count)
        {
            throw new InvalidOperationException($"{nameof(LoadStats)}: Stat types and strengths count mismatch.");
        }

        for (int i = 0; i < statTypes.Count; i++)
        {
            CharmRoll roll = new CharmRoll(statTypes[i], strengths[i]);
            appliedStats.Add(roll);
        }
    }

   public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (appliedStats.Count == 0) return;
        
        // Weapon Warning
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
        
        // Reset Layout Data
        _maxCharmLineWidth = 0f;
        _totalCharmBlockHeight = 0f;
        
        // 1. Header (Combined Name + Rarity)
        string headerText = $"{charmName} [{appliedCharmRarity}]";
        AddCharmLine(tooltips, "CharmHeader", headerText, CharmBalance.GetCharmColor(appliedCharmRarity));
        
        // 2. Stats
        for (int i = 0; i < appliedStats.Count; i++)
        {
            CharmRoll roll = appliedStats[i];
            float str = roll.GetStrength(CharmBalance.StatDisplayValue(roll.Stat));
            float perfection = CharmBalance.GetRollQualityPercentage(appliedCharmRarity, roll.Stat, roll.RawStrength);
            string text = Charm.StatTexts[roll.Stat].Format(str) + $" [{perfection}%]";
           //string text = Charm.StatTexts[roll.Stat].Format(str);
            
            AddCharmLine(tooltips, $"CharmStat_{i}", text, CharmBalance.GetStatColor(roll.Stat));
        }

        // 3. Augmentation
        if (augmentation != null)
        {
            AddCharmLine(tooltips, "CharmAugmentation", augmentation.LocalizedTooltip, Color.Lerp(Main.DiscoColor, Color.LightGray, 0.4f));
        }
    }

    private void AddCharmLine(List<TooltipLine> tooltips, string name, string text, Color color)
    {
        TooltipLine line = new TooltipLine(Mod, name, text)
        {
            OverrideColor = color
        };
        tooltips.Add(line);
        
        // Calculate Size for the Big Box
        Vector2 size = ChatManager.GetStringSize(FontAssets.MouseText.Value, text, Vector2.One);
        
        // Width: Track max width found
        if (size.X > _maxCharmLineWidth) _maxCharmLineWidth = size.X;

        // Height: Accumulate total height
        _totalCharmBlockHeight += size.Y; 
    }

    public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
    {
        if (line.Mod != Mod.Name) return true;
        
        if (line.Name == "CharmHeader")
        {
            SpriteBatch sb = Main.spriteBatch;
            
            // --- LAYOUT CALCULATIONS ---
            int paddingX = 10;
            int liftAmount = 8;  // Lift top border to clear text
            int extraBottomPad = 0;
            
            int boxX = line.X - paddingX;
            int boxY = line.Y - liftAmount;
            
            int boxWidth = (int)_maxCharmLineWidth + (paddingX * 2);
            int boxHeight = (int)_totalCharmBlockHeight + liftAmount + extraBottomPad;
            Color borderColor = CharmBalance.GetCharmColor(appliedCharmRarity);
            Color bgColor = new Color(0, 0, 0, 150); 
            
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            // 1. Uniform Background (One single rectangle)
            sb.Draw(pixel, new Rectangle(boxX, boxY, boxWidth, boxHeight), bgColor);

            // 2. Borders (Outer Frame)
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