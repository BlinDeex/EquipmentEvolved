using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.CharmsModule.Core;
using EquipmentEvolved.Assets.CharmsModule.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.CharmsModule.Items;

public class UnidentifiedEpicCharm : ModItem
{
    public override string Texture => $"{Mod.Name}/Assets/CharmsModule/Textures/Charm_NoTex";

    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 10;
    }

    public override void SetDefaults()
    {
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = 99;
        Item.consumable = true;
        Item.rare = ItemRarityID.LightRed;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips[0].OverrideColor = CharmBalance.GetCharmColor(CharmRarity.Epic);

        tooltips.Add(new TooltipLine(Mod, "IdentifyDesc", "Right-click to identify.\nContains a random Epic charm.")
        {
            OverrideColor = Color.LightGray
        });
    }

    public override bool CanRightClick() => true;

    public override void RightClick(Player player)
    {
        int type = ModContent.ItemType<Charm>();
        int index = Item.NewItem(player.GetSource_OpenItem(Item.type), player.Center, type);
        
        if (Main.item[index].ModItem is Charm charm)
        {
            CharmType randomType = (CharmType)Main.rand.Next(1, 4);
            CharmsManager.InitializeCraftedCharm(charm, CharmRarity.Epic, randomType, player);
        }
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<RareCharmDust>(10)
            .AddTile(TileID.Anvils)
            .Register();
    }
    
    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        Charm.DrawCharmShaderStatic(spriteBatch, position, 0f, scale, Main.UIScaleMatrix, GetShiftingShape(), CharmRarity.Epic);
        return false; 
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        Vector2 drawPos = Item.Center - Main.screenPosition;
        Charm.DrawCharmShaderStatic(spriteBatch, drawPos, rotation, scale, Main.GameViewMatrix.TransformationMatrix, GetShiftingShape(), CharmRarity.Epic);
        return false;
    }
    
    private CharmType GetShiftingShape()
    {
        int cycle = (int)(Main.GlobalTimeWrappedHourly / 0.75f) % 3;
        
        return cycle switch
        {
            0 => CharmType.Circle,
            1 => CharmType.Square,
            2 => CharmType.Triangle,
            _ => CharmType.Circle
        };
    }
}