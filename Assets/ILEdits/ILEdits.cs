using System;
using System.Reflection;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPlayers;
using EquipmentEvolved.Assets.Utilities;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ILEdits;

public class ILEdits : ModSystem
{
    public override void PostSetupContent()
    {
        SpelunkerEdit();
        IL_NPC.SpawnNPC += ChainedGoblinCondChange;
    }
    
    private void SpelunkerEdit()
    {
        IL_TileDrawing.DrawSingleTile += TileDrawingSpelunkerEdit;
        IL_TileDrawing.DrawAnimatedTile_AdjustForVisionChangers += TileDrawingSpelunkerEdit;
    }

    private static void TileDrawingSpelunkerEdit(ILContext il)
    {
        ILCursor c = new ILCursor(il);
        const MoveType moveType = MoveType.After;
        if (!c.TryGotoNext(moveType, x => x.Match(OpCodes.Ldc_I4, 200)))
        {
            UtilMethods.LogMessage("Failed DrawAnimatedTileSpelunkerEdit 200 1", LogType.Error);
            return;
        }
        
        if (!c.TryGotoNext(moveType, x => x.Match(OpCodes.Ldc_I4, 200)))
        {
            UtilMethods.LogMessage("Failed DrawAnimatedTileSpelunkerEdit 200 2", LogType.Error);
            return;
        }

        // red
        c.EmitDelegate((int _) =>
        {
            int revealingTicks = Main.LocalPlayer.GetModPlayer<ToolPlayer>().RevealingTicks;
            if (revealingTicks <= 0) return 200;
            return (int)(200f / PrefixBalance.REVEALING_TICKS * revealingTicks *
                         PrefixBalance.REVEALING_BRIGHTNESS_MUL);
        });

        if (!c.TryGotoNext(moveType, x => x.Match(OpCodes.Ldc_I4, 170)))
        {
            UtilMethods.LogMessage("Failed DrawAnimatedTileSpelunkerEdit 170 1", LogType.Error);
            return;
        }
        if (!c.TryGotoNext(moveType, x => x.Match(OpCodes.Ldc_I4, 170)))
        {
            UtilMethods.LogMessage("Failed DrawAnimatedTileSpelunkerEdit 170 2", LogType.Error);
            return;
        }

        // green
        c.EmitDelegate((int _) =>
        {
            int revealingTicks = Main.LocalPlayer.GetModPlayer<ToolPlayer>().RevealingTicks;
            if (revealingTicks <= 0) return 170;
            return (int)(170f / PrefixBalance.REVEALING_TICKS * revealingTicks *
                         PrefixBalance.REVEALING_BRIGHTNESS_MUL);
        });
    }
    
    private void ChainedGoblinCondChange(ILContext il)
    {
        ILCursor c = new(il);
    
        // 1. Find the first location (downedGoblins)
        if (!c.TryGotoNext(MoveType.After, 
                x => x.MatchLdsfld(typeof(NPC).GetField("downedGoblins", BindingFlags.Public | BindingFlags.Static)!)))
        {
            UtilMethods.LogMessage("ChainedGoblinCondChange: Could not find 'downedGoblins' instruction.", LogType.Error);
            return;
        }
        
        ILCursor searcher = c.Clone();
        int kIndex = -1;

        // Check if we can find the local variable index
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
        
            return player.active && player.GetModPlayer<PrefixPlayer>().HasReforgedItemInv;
        });
    }
    /*
    private void EditCanRoll(ILContext il)
    {
        var c = new ILCursor(il);
        if (!c.TryGotoNext(MoveType.Before, x => x.MatchCall(typeof(Item).GetMethod("GetVanillaPrefixes")!)))
        {
            UtilMethods.LogMessage("Failed EditCanRoll GetVanillaPrefixes!", LogType.Error);
            return;
        }
        
        c.Index -= 10;

        c.EmitLdcI4(0);
        c.EmitRet();
    }
    */
}