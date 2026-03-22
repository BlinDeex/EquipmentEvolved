using EquipmentEvolved.Assets.Core;
using Terraria;

namespace EquipmentEvolved.Assets.ModPrefixes.Core;

/// <summary>
/// Used for prefixes which application order matters
/// </summary>
public interface ISequentialPrefix
{
    /// <summary>
    /// The lower the number, the higher the priority, non sequential prefixes have priority of 0
    /// </summary>
    int ExecutionPriority { get; } 
    
    /// <summary>
    /// Will be called before all other prefixes with lower priority
    /// </summary>
    void ApplySequentialStats(Player player, StatPlayer stats);
}