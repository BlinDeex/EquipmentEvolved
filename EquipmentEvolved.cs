using System.IO;
using EquipmentEvolved.Assets.CharmsModule.Core;
using EquipmentEvolved.Assets.CrossMod;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.Utilities.Spritebatch;
using Terraria.ModLoader;

namespace EquipmentEvolved;

public class EquipmentEvolved : Mod
{
    public static LogType LOG_LEVEL = LogType.Warning; // TODO: add this to config
    public static EquipmentEvolved Instance { get; private set; }

    public override void Load()
    {
        Instance = this;
        CharmsManager.Load();
    }

    public override void Unload()
    {
        SpriteBatchSnapshotCache.Unload();
        Instance = null;
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        Networking.Instance.HandlePacket(reader, whoAmI);
    }

    public override object Call(params object[] args)
    {
        return CrossModManager.HandleCall(args);
    }

    /*
    private void RebalanceExample()
    {
        if (ModLoader.TryGetMod("EquipmentEvolved", out Mod equipmentEvolved))
        {
            Dictionary<string, object> changes = new()
            {
                { "ACCESSORY_REFORGING_MULTIPLIER", 2.24f },
                { "WEAPON_REFORGING_MULTIPLIER", "2.24" },
                { "EFFICIENT_MANA_SAVED", 2 }
            };

            string result = (string)equipmentEvolved.Call("Rebalance", changes);
            Logger.Info(result);
        }
    }
    */
}