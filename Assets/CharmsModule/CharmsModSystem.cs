using System;
using EquipmentEvolved.Assets.CharmsModule.Items;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.CharmsModule;

public class CharmsModSystem : ModSystem
{

    public override void Load()
    {

        On_PopupText.NewText_PopupTextContext_Item_int_bool_bool += (orig, context, item, stack, noStack, text) =>
        {
            if (item.type != ModContent.ItemType<Charm>())
            {
                return orig.Invoke(context, item, stack, noStack, text);
            }
            
            if (!Main.showItemText)
                return -1;
            
            if (item.Name == null || !item.active)
                return -1;
            
            if (Main.netMode == NetmodeID.Server)
                return -1;
            
            Charm charmItem = (Charm)item.ModItem;
            
            AdvancedPopupRequest req = new()
            {
                Color = charmItem.CharmColor,
                DurationInFrames = 130,
                Text = charmItem.CharmName,
                Velocity = new Vector2(Main.rand.NextFloat() * 5f, -10f)
            };

            PopupText.NewText(req, item.position);
            
            return FindNextItemTextSlot();
        };

        IL_Main.MouseTextInner += il => //TODO: Detour/IL ChatManager.DrawColorCodedStringWithShadow directly so I can apply shaders in the future
        {
            ILCursor c = new(il)
            {
                Index = il.Instrs.Count
            };
            
            c.GotoPrev(MoveType.Before, x => x.MatchLdcR4(0.0f));
            c.EmitDelegate<Func<Color, Color>>(color =>
            {
                Color targetColor = color;
                if (TryGetMouseHoverItem(out Item item) && item.ModItem is Charm charm)
                {
                    targetColor = charm.CharmColor;
                }
                return targetColor;
            });
        };
    }

    private static bool TryGetMouseHoverItem(out Item item) //TODO: if there will be zoom issues check here first
    {
        item = null;
        PlayerInput.SetZoom_Unscaled();
        PlayerInput.SetZoom_MouseInWorld();
        Rectangle mouseRectangle = new Rectangle((int)(Main.mouseX + Main.screenPosition.X), (int)(Main.mouseY + Main.screenPosition.Y), 1, 1);
        PlayerInput.SetZoom_UI();
        
        if (Math.Abs(Main.LocalPlayer.gravDir - -1f) < float.Epsilon)
            mouseRectangle.Y = (int)Main.screenPosition.Y + Main.screenHeight - Main.mouseY;
        
        for (int i = 0; i < 400; i++) 
        {
            if (!Main.item[i].active)
                continue;

            Rectangle drawHitbox = Item.GetDrawHitbox(Main.item[i].type, Main.LocalPlayer);
            Vector2 bottom = Main.item[i].Bottom;
            Rectangle value = new Rectangle((int)(bottom.X - drawHitbox.Width * 0.5f), (int)(bottom.Y - drawHitbox.Height), drawHitbox.Width, drawHitbox.Height);
            if (!mouseRectangle.Intersects(value)) continue;
            item = Main.item[i];
            return true;
        }
        
        return false;
    }
    
    private static int FindNextItemTextSlot()
    {
        int num = -1;
        for (int i = 0; i < 20; i++)
        {
            if (Main.popupText[i].active) continue;
            num = i;
            break;
        }

        if (num != -1) return num;
        
        double num2 = Main.bottomWorld;
        for (int j = 0; j < 20; j++) 
        {
            if (num2 > Main.popupText[j].position.Y) 
            {
                num = j;
                num2 = Main.popupText[j].position.Y;
            }
        }

        return num;
    }
}