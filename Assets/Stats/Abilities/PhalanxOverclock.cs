using EquipmentEvolved.Assets.Core;

namespace EquipmentEvolved.Assets.Stats.Abilities;

public class PhalanxOverclock : EquipmentStat
{
    public override StatStackingMode StackingMode => StatStackingMode.Max;
    public override bool HiddenFromDebugAccessory => true;
    
    public override string FormatTooltip(float totalValue) => ""; 
}