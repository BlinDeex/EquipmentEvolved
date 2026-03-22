using System;
using System.Linq;
using EquipmentEvolved.Assets.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.DEBUG.Commands;

public class TempStatCommand : ModCommand
{
    public override CommandType Type => CommandType.Chat;

    public override string Command => "tempstat";
    public override string Usage => "/tempstat <StatName> <Strength> <DurationInSeconds>";

    public override string Description => "Grants a temporary EquipmentStat to the player for debugging.";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (args.Length != 3)
        {
            Main.NewText($"Usage: {Usage}", Color.Red);
            return;
        }

        string statName = args[0];

        if (!float.TryParse(args[1], out float strength))
        {
            Main.NewText($"Invalid strength value: '{args[1]}'. Must be a number.", Color.Red);
            return;
        }
        
        if (!float.TryParse(args[2], out float durationSeconds))
        {
            Main.NewText($"Invalid duration value: '{args[2]}'. Must be a number of seconds.", Color.Red);
            return;
        }

        EquipmentStat targetStat = ModContent.GetContent<EquipmentStat>()
            .FirstOrDefault(s => s.Name.Equals(statName, StringComparison.OrdinalIgnoreCase));

        if (targetStat == null)
        {
            Main.NewText($"Could not find an EquipmentStat named '{statName}'.", Color.Red);
            return;
        }
        
        int durationTicks = (int)Math.Round(durationSeconds * 60f);

        StatPlayer statPlayer = caller.Player.GetModPlayer<StatPlayer>();
        
        statPlayer.AddTemporaryStat(
            stat: targetStat,
            value: strength,
            durationTicks: durationTicks,
            mode: StatReapplicationMode.Independent,
            source: StatSource.Generic
        );
        Main.NewText($"Successfully applied {targetStat.Name} (Strength: {strength}) for {durationSeconds} seconds.", Color.Green);
    }
}