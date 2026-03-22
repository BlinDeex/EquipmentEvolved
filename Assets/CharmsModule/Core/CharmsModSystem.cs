using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.CharmsModule.Data;
using EquipmentEvolved.Assets.CharmsModule.Items;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace EquipmentEvolved.Assets.CharmsModule.Core;

public class CharmsModSystem : ModSystem
{
    private static HashSet<string> _charmNames;
    private static Effect _charmShader;

    private Hook _drawSnippetsHook;
    private Hook _drawStringHook;
    private static FieldInfo _transformMatrixField;
    
    private static Color? _worldHoverCharmColor;
    private static Color[] _securePopupColors;

    public override void PostSetupContent()
    {
        _charmNames = new HashSet<string>();
        foreach (CharmName nameEnum in Enum.GetValues(typeof(CharmName)))
        {
            // Removed the hardcoded + " Charm" postfix here!
            _charmNames.Add(CharmBalance.SplitCamelCase(nameEnum));
        }
        _charmNames.Add("Unidentified");

        UtilMethods.LogMessage($"Registered {_charmNames.Count} Charm names for text shaders.", LogType.Debug);

        if (Main.netMode == NetmodeID.Server) return;

        try
        {
            _charmShader = ModContent.Request<Effect>("EquipmentEvolved/Assets/Effects/Charms/CharmTextShader", AssetRequestMode.ImmediateLoad).Value;
        }
        catch (Exception ex)
        {
            UtilMethods.LogMessage($"Failed to load CharmTextShader: {ex.Message}", LogType.Error);
        }
    }

    public override void Load()
    {
        _securePopupColors = new Color[20];
        
        On_Main.DrawItemTextPopups += On_MainOnDrawItemTextPopups;
        On_PopupText.NewText_PopupTextContext_Item_int_bool_bool += InjectCharmPopupColor;

        _transformMatrixField = typeof(SpriteBatch).GetField("transformMatrix", BindingFlags.Instance | BindingFlags.NonPublic);
        
        if (_transformMatrixField == null)
        {
            UtilMethods.LogMessage("SpriteBatch transformMatrix field not found! Matrix fallbacks will be used.", LogType.Warning);
        }

        MethodInfo snippetMethod = typeof(ChatManager).GetMethod("DrawColorCodedString", BindingFlags.Static | BindingFlags.Public, [typeof(SpriteBatch), typeof(DynamicSpriteFont), typeof(TextSnippet[]), typeof(Vector2), typeof(Color), typeof(float), typeof(Vector2), typeof(Vector2), typeof(int).MakeByRefType(), typeof(float), typeof(bool)]);
        MethodInfo stringMethod = typeof(ChatManager).GetMethod("DrawColorCodedString", BindingFlags.Static | BindingFlags.Public, [typeof(SpriteBatch), typeof(DynamicSpriteFont), typeof(string), typeof(Vector2), typeof(Color), typeof(float), typeof(Vector2), typeof(Vector2), typeof(float), typeof(bool)]);

        if (snippetMethod != null)
        {
            _drawSnippetsHook = new Hook(snippetMethod, typeof(CharmsModSystem).GetMethod(nameof(ApplyShaderToSnippets), BindingFlags.Static | BindingFlags.NonPublic)!);
        }
        else
        {
            UtilMethods.LogMessage("DrawColorCodedString (Snippets) method not found! Hover text shaders may fail.", LogType.Error);
        }

        if (stringMethod != null)
        {
            _drawStringHook = new Hook(stringMethod, typeof(CharmsModSystem).GetMethod(nameof(ApplyShaderToString), BindingFlags.Static | BindingFlags.NonPublic)!);
        }
        else
        {
            UtilMethods.LogMessage("DrawColorCodedString (String) method not found! Tooltip shaders may fail.", LogType.Error);
        }
    }

    public override void Unload()
    {
        _charmNames = null;
        _charmShader = null;
        _transformMatrixField = null;
        _securePopupColors = null;

        _drawSnippetsHook?.Dispose();
        _drawStringHook?.Dispose();
    }

    public override void UpdateUI(GameTime gameTime)
    {
        _worldHoverCharmColor = null;
        if (Main.netMode == NetmodeID.Server) return;

        if (TryGetMouseHoverItem(out Item item))
        {
            if (item.ModItem is Charm charm)
            {
                _worldHoverCharmColor = charm.CharmColor;
                
                // Strip it from the in-world mouse text
                if (!string.IsNullOrEmpty(Main.hoverItemName) && Main.hoverItemName.Contains(" Charm"))
                {
                    Main.hoverItemName = Main.hoverItemName.Replace(" Charm", "");
                }
            }
            // NEW: Feed strict colors for the Unidentified Charms!
            else if (item.ModItem is UnidentifiedRareCharm)
            {
                _worldHoverCharmColor = CharmBalance.GetCharmColor(CharmRarity.Rare);
            }
            else if (item.ModItem is UnidentifiedEpicCharm)
            {
                _worldHoverCharmColor = CharmBalance.GetCharmColor(CharmRarity.Epic);
            }
        }
    }

    private static int InjectCharmPopupColor(On_PopupText.orig_NewText_PopupTextContext_Item_int_bool_bool orig, PopupTextContext context, Item item, int stack, bool noStack, bool text)
    {
        int index = orig(context, item, stack, noStack, text);
    
        if (index != -1 && item.ModItem is Charm charm)
        {
            _securePopupColors[index] = charm.CharmColor;
            Main.popupText[index].name = charm.CharmName; 
        }
    
        return index;
    }

    private static void On_MainOnDrawItemTextPopups(On_Main.orig_DrawItemTextPopups orig, float scaleTarget)
    {
        if (_charmShader == null)
        {
            orig(scaleTarget);
            return;
        }

        bool[] wasActive = new bool[20];
        bool[] isCharm = new bool[20];
        bool hasCharms = false;
        
        for (int i = 0; i < 20; i++)
        {
            wasActive[i] = Main.popupText[i].active;
            if (wasActive[i] && !string.IsNullOrEmpty(Main.popupText[i].name) && IsCharmText(Main.popupText[i].name))
            {
                isCharm[i] = true;
                hasCharms = true;
            }
        }

        if (!hasCharms)
        {
            orig(scaleTarget);
            return;
        }
        
        for (int i = 0; i < 20; i++)
        {
            if (isCharm[i]) Main.popupText[i].active = false;
        }

        orig(scaleTarget);
        
        Vector2[] originalPositions = new Vector2[20];
        float t = Main.GlobalTimeWrappedHourly * 60f;
        Vector2 exaltedShake = new Vector2((float)Math.Sin(t * 1.5f) * 2.5f, (float)Math.Cos(t * 2.2f) * 2.5f);

        for (int i = 0; i < 20; i++)
        {
            if (isCharm[i] && wasActive[i])
            {
                Main.popupText[i].active = true;
                Main.popupText[i].color = _securePopupColors[i];

                originalPositions[i] = Main.popupText[i].position;
                if (_securePopupColors[i] == new Color(0, 255, 0))
                {
                    Main.popupText[i].position += exaltedShake;
                }
            }
            else
            {
                Main.popupText[i].active = false;
            }
        }
        
        Matrix currentMatrix = _transformMatrixField?.GetValue(Main.spriteBatch) as Matrix? ?? Main.GameViewMatrix.ZoomMatrix;

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise,
            _charmShader, currentMatrix);
        _charmShader.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly);

        orig(scaleTarget);

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null,
            currentMatrix);
        
        for (int i = 0; i < 20; i++)
        {
            Main.popupText[i].active = wasActive[i];
            if (isCharm[i])
            {
                Main.popupText[i].position = originalPositions[i];
            }
        }
    }

    private delegate Vector2 orig_DrawSnippets(SpriteBatch spriteBatch, DynamicSpriteFont font, TextSnippet[] snippets, Vector2 position, Color baseColor,
        float rotation, Vector2 origin, Vector2 baseScale, out int hoveredSnippet, float maxWidth, bool ignoreColors);
    
    private delegate Vector2 orig_DrawString(SpriteBatch spriteBatch, DynamicSpriteFont font, string text, Vector2 position, Color baseColor, float rotation, Vector2 origin,
        Vector2 baseScale, float maxWidth, bool ignoreColors);

    private static Vector2 ApplyShaderToSnippets(orig_DrawSnippets orig, SpriteBatch spriteBatch, DynamicSpriteFont font, TextSnippet[] snippets,
        Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, out int hoveredSnippet, float maxWidth, bool ignoreColors)
{
    string fullText = string.Empty;
    if (snippets != null)
    {
        foreach (TextSnippet snippet in snippets) fullText += snippet.Text;
    }

    if (!IsCharmText(fullText)) return orig(spriteBatch, font, snippets, position, baseColor, rotation, origin, baseScale, out hoveredSnippet, maxWidth, ignoreColors);

    Vector2 drawPosition = position;
    bool isExalted = false;
    if (_worldHoverCharmColor.HasValue && _worldHoverCharmColor.Value == new Color(0, 255, 0)) isExalted = true;
    else if (baseColor == new Color(0, 255, 0)) isExalted = true;
    else if (snippets != null && snippets.Length > 0 && snippets[0].Color == new Color(0, 255, 0)) isExalted = true;

    if (isExalted)
    {
        float t = Main.GlobalTimeWrappedHourly * 60f;
        drawPosition += new Vector2((float)Math.Sin(t * 1.5f) * 2.5f, (float)Math.Cos(t * 2.2f) * 2.5f);
    }

    if (_worldHoverCharmColor.HasValue && !ignoreColors)
    {
        if (snippets != null)
        {
            foreach (TextSnippet snippet in snippets)
            {
                snippet.Color = _worldHoverCharmColor.Value;
            }
        }
    }

    bool applyShader = !ignoreColors && _charmShader != null;
    if (!applyShader) return orig(spriteBatch, font, snippets, drawPosition, baseColor, rotation, origin, baseScale, out hoveredSnippet, maxWidth, ignoreColors);
    
    Matrix currentMatrix = _transformMatrixField?.GetValue(spriteBatch) as Matrix? ?? Main.UIScaleMatrix;
    
    spriteBatch.End();
    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, _charmShader, currentMatrix);
    _charmShader.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly);

    Vector2 result = orig(spriteBatch, font, snippets, drawPosition, baseColor, rotation, origin, baseScale, out hoveredSnippet, maxWidth, false);
    
    spriteBatch.End();
    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, currentMatrix);

    return result;
}

    private static Vector2 ApplyShaderToString(orig_DrawString orig, SpriteBatch spriteBatch, DynamicSpriteFont font, string text, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth, bool ignoreColors)
    {
        if (!IsCharmText(text)) return orig(spriteBatch, font, text, position, baseColor, rotation, origin, baseScale, maxWidth, ignoreColors);
    
        Color drawColor = _worldHoverCharmColor ?? baseColor;
        Vector2 drawPosition = position;
        
        if (drawColor == new Color(0, 255, 0))
        {
            float t = Main.GlobalTimeWrappedHourly * 60f;
            drawPosition += new Vector2((float)Math.Sin(t * 1.5f) * 2.5f, (float)Math.Cos(t * 2.2f) * 2.5f);
        }

        if (_worldHoverCharmColor.HasValue && !ignoreColors)
        {
            float pulse = Main.mouseTextColor / 255f;
            drawColor = _worldHoverCharmColor.Value * pulse;
            drawColor.A = Main.mouseTextColor;
        }

        bool applyShader = !ignoreColors && _charmShader != null;
        if (!applyShader) return orig(spriteBatch, font, text, drawPosition, drawColor, rotation, origin, baseScale, maxWidth, ignoreColors);

        Matrix currentMatrix = _transformMatrixField?.GetValue(spriteBatch) as Matrix? ?? Main.UIScaleMatrix;

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, _charmShader, currentMatrix);
        _charmShader.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly);

        Vector2 result = orig(spriteBatch, font, text, drawPosition, drawColor, rotation, origin, baseScale, maxWidth, false);
    
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, currentMatrix);

        return result;
    }

    private static bool IsCharmText(string text)
    {
        if (string.IsNullOrEmpty(text) || _charmNames == null) return false;

        return _charmNames.Any(text.StartsWith);
    }

    private static bool TryGetMouseHoverItem(out Item item)
    {
        item = null;
        PlayerInput.SetZoom_Unscaled();
        PlayerInput.SetZoom_MouseInWorld();
        Rectangle mouseRectangle = new((int)(Main.mouseX + Main.screenPosition.X), (int)(Main.mouseY + Main.screenPosition.Y), 1, 1);
        PlayerInput.SetZoom_UI();

        if (Math.Abs(Main.LocalPlayer.gravDir - -1f) < float.Epsilon) mouseRectangle.Y = (int)Main.screenPosition.Y + Main.screenHeight - Main.mouseY;

        for (int i = 0; i < 400; i++)
        {
            if (!Main.item[i].active) continue;

            Rectangle drawHitbox = Item.GetDrawHitbox(Main.item[i].type, Main.LocalPlayer);
            Vector2 bottom = Main.item[i].Bottom;
            Rectangle value = new((int)(bottom.X - drawHitbox.Width * 0.5f), (int)(bottom.Y - drawHitbox.Height), drawHitbox.Width, drawHitbox.Height);
            if (!mouseRectangle.Intersects(value)) continue;

            item = Main.item[i];
            return true;
        }

        return false;
    }
}