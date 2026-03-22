using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.CharmsModule.Core;
using EquipmentEvolved.Assets.CharmsModule.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.CharmsModule.Items;

public class CommonCharmDust : ModItem
{
    public override string Texture => $"{Mod.Name}/Assets/CharmsModule/Textures/Charm_NoTex";

    public override void SetDefaults()
    {
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = 9999;
        Item.value = Item.sellPrice(copper: 50);
        Item.rare = ItemRarityID.White;
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        Color color = CharmBalance.GetCharmColor(CharmRarity.Common); // Use .Rare for the Epic dust!
        
        // Item.type is always the exact same number, perfect for inventory!
        DustManager.DrawDustShaderStatic(spriteBatch, position, color, scale, Main.UIScaleMatrix, Item.type);
        
        return false; 
    }

    // Notice we are using 'int whoAmI' from the method parameters!
    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        Vector2 drawPos = Item.Center - Main.screenPosition;
        Color color = CharmBalance.GetCharmColor(CharmRarity.Common); // Use .Rare for the Epic dust!
        
        // whoAmI is the item's static entity ID in the world. It never changes while falling!
        float safeSeed = whoAmI;
        
        DustManager.DrawDustShaderStatic(spriteBatch, drawPos, color, scale, Main.GameViewMatrix.TransformationMatrix, safeSeed);
        
        return false;
    }
}