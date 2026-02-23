using System.Collections.Generic;
using EquipmentEvolved.Assets.CharmsModule;
using EquipmentEvolved.Assets.CharmsModule.Manager;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.DEBUG.Commands;

public class SpawnCharmsCommand : ModCommand
{
    private static readonly Dictionary<string, CharmRarity> rarities = new()
    {
        { "common", CharmRarity.Common },
        { "rare", CharmRarity.Rare },
        { "epic", CharmRarity.Epic },
        { "legendary", CharmRarity.Legendary },
        { "mythical", CharmRarity.Mythical },
        { "exalted", CharmRarity.Exalted }
    };
    
    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (args.Length != 2)
        {
            caller.Reply($"Invalid amount of args! expected 2, got {args.Length}");
            return;
        }

        string rarityArg = args[0];
        string countArg = args[1];

        if (!rarities.TryGetValue(rarityArg, out CharmRarity rarity))
        {
            caller.Reply($"{args[0]} rarity doesnt exist! Valid rarities: common rare epic legendary mythical");
            return;
        }

        if (!int.TryParse(countArg, out int countArgInt))
        {
            caller.Reply($"Second arg is supposed to be a number! Got <{args[1]}> instead");
            return;
        }

        for (int i = 0; i < countArgInt; i++)
        {
            CharmsManager.SpawnCharms([(rarity, CharmsManager.RollCharmType())], caller.Player.whoAmI, spawnPos: caller.Player.Center);
        }

        string charmCharms = countArg.EndsWith('1') ? "charm" : "charms";
        caller.Reply($"Successfully spawned {countArgInt} {rarity.ToString()} {charmCharms}!");
    }

    public override string Command => "SpawnCharm";
    public override CommandType Type => CommandType.Chat;
    public override string Usage => "</SpawnCharm Rare 4> </SpawnCharm Mythical 10> </SpawnCharm Common 1>";
}