using EquipmentEvolved.Assets.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.DEBUG.Commands;

public class ClearTempStatsCommand : ModCommand
{
    public override CommandType Type => CommandType.Chat;
    
    public override string Command => "cleartempstats";

    public override string Usage => "/cleartempstats";

    public override string Description => "Instantly removes all active temporary EquipmentStats from the player.";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        StatPlayer statPlayer = caller.Player.GetModPlayer<StatPlayer>();
        statPlayer.ClearTemporaryStats();

        Main.NewText("Successfully cleared all temporary stats.", Color.Green);
    }
}