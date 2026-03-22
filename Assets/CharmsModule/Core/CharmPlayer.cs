using EquipmentEvolved.Assets.CharmsModule.Data;
using EquipmentEvolved.Assets.CharmsModule.Items;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace EquipmentEvolved.Assets.CharmsModule.Core;

public class CharmPlayer : ModPlayer
{
    public int LegendaryPity { get; private set; }
    public int MythicalPity { get; private set; }
    
    public bool HasSeenConverterMessage { get; set; } = false;
    
    public override void SaveData(TagCompound tag)
    {
        tag.Add(nameof(LegendaryPity), LegendaryPity);
        tag.Add(nameof(MythicalPity), MythicalPity);
        tag.Add(nameof(HasSeenConverterMessage), HasSeenConverterMessage);
    }

    public override void LoadData(TagCompound tag)
    {
        LegendaryPity = tag.ContainsKey(nameof(LegendaryPity)) ? tag.GetInt(nameof(LegendaryPity)) : 0;
        MythicalPity = tag.ContainsKey(nameof(MythicalPity)) ? tag.GetInt(nameof(MythicalPity)) : 0;
        HasSeenConverterMessage = tag.GetBool(nameof(HasSeenConverterMessage));
    }

    public void IncreasePity()
    {
        LegendaryPity++;
        MythicalPity++;
    }

    public void ResetPity(CharmRarity rarity, bool quiet = false)
    {
        LegendaryPity = 0;
        string text = "Legendary";
        if (rarity == CharmRarity.Mythical)
        {
            MythicalPity = 0;
            text += " and Mythical";
        }

        text += $" pity has been reset for {Player.name}!";

        if (quiet) return;

        UtilMethods.BroadcastOrNewText(text, Color.GreenYellow);
    }
    
    public override void PostUpdateMiscEffects()
    {
        // If they already saw it, do nothing!
        if (HasSeenConverterMessage) return;

        // Only run this logic on the local client to prevent multiplayer spam
        if (Player.whoAmI != Main.myPlayer) return;

        int junkCharmCount = 0;

        // Scan the main 50 inventory slots
        for (int i = 0; i < 50; i++)
        {
            Item item = Player.inventory[i];
            
            if (!item.IsAir && item.ModItem is Charm { CharmRarity: CharmRarity.Common or CharmRarity.Rare })
            {
                junkCharmCount++;
            }
        }

        if (junkCharmCount >= 10)
        {
            HasSeenConverterMessage = true;

            Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuOpen);

            // Fetch the text from your localization files!
            Main.NewText(Language.GetTextValue("Mods.EquipmentEvolved.UI.ConverterNotice.Line1"), new Color(255, 200, 50));
            Main.NewText(Language.GetTextValue("Mods.EquipmentEvolved.UI.ConverterNotice.Line2"), Microsoft.Xna.Framework.Color.LightGray);
            Main.NewText(Language.GetTextValue("Mods.EquipmentEvolved.UI.ConverterNotice.Line3"), Microsoft.Xna.Framework.Color.LightGray);
        }
    }
}