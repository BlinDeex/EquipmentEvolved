using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace EquipmentEvolved.Assets.CharmsModule;

public class CharmPlayer : ModPlayer
{
    public int LegendaryPity { get; private set; }
    public int MythicalPity { get; private set; }

    public override void SaveData(TagCompound tag)
    {
        tag.Add(nameof(LegendaryPity), LegendaryPity);
        tag.Add(nameof(MythicalPity), MythicalPity);
    }

    public override void LoadData(TagCompound tag)
    {
        LegendaryPity = tag.ContainsKey(nameof(LegendaryPity)) ? tag.GetInt(nameof(LegendaryPity)) : 0;
        MythicalPity = tag.ContainsKey(nameof(MythicalPity)) ? tag.GetInt(nameof(MythicalPity)) : 0;
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
}