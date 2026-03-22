using System.Collections.Generic;
using EquipmentEvolved.Assets.CharmsModule.Data;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace EquipmentEvolved.Assets.CharmsModule.Items;

public class CharmConverter : ModItem
{
    public bool ConvertCommon { get; set; }
    public bool ConvertRare { get; set; }
    
    public override string Texture => $"Terraria/Images/Item_{ItemID.ExtendoGrip}"; 

    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 28;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(gold: 1);
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.autoReuse = false;
    }
    
    public override bool AltFunctionUse(Player player) => true; 

    public override bool CanUseItem(Player player)
    {
        if (player.altFunctionUse == 2)
        {
            ConvertRare = !ConvertRare;
            Main.NewText($"Rare Charm Conversion: {(ConvertRare ? "[c/00FF00:ON]" : "[c/FF0000:OFF]")}");
        }
        else
        {
            ConvertCommon = !ConvertCommon;
            Main.NewText($"Common Charm Conversion: {(ConvertCommon ? "[c/00FF00:ON]" : "[c/FF0000:OFF]")}");
        }
        
        return true;
    }
    
    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.GoldBar, 5)
            .AddIngredient(ItemID.Wire, 10)
            .AddTile(TileID.Anvils)
            .Register();
        
        CreateRecipe()
            .AddIngredient(ItemID.PlatinumBar, 5)
            .AddIngredient(ItemID.Wire, 10)
            .AddTile(TileID.Anvils)
            .Register();
    }

    public override void UpdateInventory(Player player)
    {
        if (!ConvertCommon && !ConvertRare) return;

        bool playedSound = false;
        
        for (int i = 0; i < 50; i++) 
        {
            Item invItem = player.inventory[i];
            
            if (invItem.IsAir || invItem.ModItem is not Charm charm) continue;

            if (ConvertCommon && charm.CharmRarity == CharmRarity.Common)
            {
                invItem.TurnToAir();
                player.QuickSpawnItem(player.GetSource_Misc("CharmConversion"), ModContent.ItemType<CommonCharmDust>(), 1);
                playedSound = true;
            }
            else if (ConvertRare && charm.CharmRarity == CharmRarity.Rare)
            {
                invItem.TurnToAir();
                player.QuickSpawnItem(player.GetSource_Misc("CharmConversion"), ModContent.ItemType<RareCharmDust>(), 1);
                playedSound = true;
            }
        }
        
        if (playedSound)
        {
            SoundEngine.PlaySound(SoundID.Grab, player.Center);
        }
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.Add(new TooltipLine(Mod, "ConverterCommon", $"Common Conversion: {(ConvertCommon ? "[c/00FF00:ON]" : "[c/FF0000:OFF]")}"));
        tooltips.Add(new TooltipLine(Mod, "ConverterRare", $"Rare Conversion: {(ConvertRare ? "[c/00FF00:ON]" : "[c/FF0000:OFF]")}"));
        
        tooltips.Add(new TooltipLine(Mod, "ConverterInstructions", "Hold and Left-click to toggle Common.\nHold and Right-click to toggle Rare.")
        {
            OverrideColor = Microsoft.Xna.Framework.Color.Gray
        });
    }

    public override void SaveData(TagCompound tag)
    {
        tag.Add(nameof(ConvertCommon), ConvertCommon);
        tag.Add(nameof(ConvertRare), ConvertRare);
    }

    public override void LoadData(TagCompound tag)
    {
        ConvertCommon = tag.GetBool(nameof(ConvertCommon));
        ConvertRare = tag.GetBool(nameof(ConvertRare));
    }
}