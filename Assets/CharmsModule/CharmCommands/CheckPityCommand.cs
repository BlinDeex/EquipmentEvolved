using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.CharmsModule.CharmCommands;

public class CheckPityCommand : ModCommand
{
    public override void Action(CommandCaller caller, string input, string[] args)
    {
        CharmPlayer charmPlayer = caller.Player.GetModPlayer<CharmPlayer>();
        string text =
            $"Legendary pity: {charmPlayer.LegendaryPity}/{CharmBalance.LegendaryPity} | Mythical pity: {charmPlayer.MythicalPity}/{CharmBalance.MythicalPity}";
        caller.Reply(text, Color.YellowGreen);
    }

    public override string Command => "CheckPity";
    public override CommandType Type => CommandType.Chat;
}