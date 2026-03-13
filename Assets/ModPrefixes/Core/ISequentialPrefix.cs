using EquipmentEvolved.Assets.Core;
using Terraria;

namespace EquipmentEvolved.Assets.ModPrefixes.Core;

public interface ISequentialPrefix
{
    /// <summary>
    /// The lower the number, the higher the priority
    /// </summary>
    int ExecutionPriority { get; } 
    
    void ApplySequentialStats(Player player, StatPlayer stats);
}