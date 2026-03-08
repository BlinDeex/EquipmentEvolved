using System;
using System.Reflection;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Pickaxe.Revealing;
using EquipmentEvolved.Assets.Utilities;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Patches;

public class ILEdits : ModSystem
{
    public override void PostSetupContent()
    {
        SpelunkerEdit();
        IL_NPC.SpawnNPC += ChainedGoblinCondChange;
    }

    private static void SpelunkerEdit()
    {
        IL_TileDrawing.DrawSingleTile += TileDrawingSpelunkerEdit;
        IL_TileDrawing.DrawAnimatedTile_AdjustForVisionChangers += TileDrawingSpelunkerEdit;
    }

    private static void TileDrawingSpelunkerEdit(ILContext il)
    {
        ILCursor c = new(il);

        try
        {
            if (!c.TryGotoNext(MoveType.After, x => x.MatchCall<Main>("IsTileSpelunkable")))
            {
                UtilMethods.LogMessage("Failed SpelunkerEdit: Could not find 'IsTileSpelunkable' anchor.", LogType.Error);
                return;
            }
            
            for (int i = 0; i < 4; i++)
            {
                if (!c.TryGotoNext(MoveType.After, x => x.MatchLdcI4(out _)))
                {
                    UtilMethods.LogMessage($"Failed SpelunkerEdit: Could not find color integer load #{i}.", LogType.Error);
                    return;
                }
                
                c.EmitDelegate<Func<int, int>>(originalColorValue =>
                {
                    int revealingTicks = Main.LocalPlayer.GetModPlayer<RevealingModPlayer>().RevealingTicks;
                    
                    if (revealingTicks <= 0) return originalColorValue;
                    
                    return (int)(originalColorValue / (float)PrefixBalance.REVEALING_TICKS * revealingTicks * PrefixBalance.REVEALING_BRIGHTNESS_MUL);
                });
            }
        }
        catch (Exception e)
        {
            UtilMethods.LogMessage($"SpelunkerEdit crashed during IL patching: {e.Message}", LogType.Error);
        }
    }

    private void ChainedGoblinCondChange(ILContext il)
    {
        ILCursor c = new(il);
        
        if (!c.TryGotoNext(MoveType.After, x => x.MatchLdsfld(typeof(NPC).GetField("downedGoblins", BindingFlags.Public | BindingFlags.Static)!)))
        {
            UtilMethods.LogMessage("ChainedGoblinCondChange: Could not find 'downedGoblins' instruction.", LogType.Error);
            return;
        }

        ILCursor searcher = c.Clone();
        int kIndex = -1;
        
        if (!searcher.TryGotoNext(x => x.MatchLdloc(out kIndex)))
        {
            UtilMethods.LogMessage("ChainedGoblinCondChange: Could not find 'k' local index.", LogType.Error);
            return;
        }

        c.Emit(OpCodes.Pop);
        c.Emit(OpCodes.Ldloc, kIndex);
        c.EmitDelegate<Func<int, bool>>(playerIndex =>
        {
            if (playerIndex is < 0 or >= Main.maxPlayers) return false;

            Player player = Main.player[playerIndex];

            return player.active && player.GetModPlayer<CorePrefixModPlayer>().HasReforgedItemInv;
        });
    }
}