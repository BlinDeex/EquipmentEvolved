using EquipmentEvolved.Assets.Misc;
using Terraria;

namespace EquipmentEvolved.Assets.CharmsModule.Augmentations;

public abstract class AugmentationBase
{
    public abstract string LocalizedTooltip { get; }
    
    public abstract void FlagEnabler(Player player);

    public abstract void FlagDisabler(Player player);

    public virtual SpecializedPrefixType PossiblePrefixTypesToApplyOn => SpecializedPrefixType.Any;
}