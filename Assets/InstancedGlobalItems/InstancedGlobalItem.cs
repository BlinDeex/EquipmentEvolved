using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.InstancedGlobalItems;

public class InstancedGlobalItem : GlobalItem
{
    public override bool InstancePerEntity => true;
    
    /// <summary>
    /// Is this item a fortune drop?
    /// </summary>
    public bool FortuneDrop { get; set; }
}