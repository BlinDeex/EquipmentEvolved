using EquipmentEvolved.Assets.CharmsModule.Items;
using EquipmentEvolved.Assets.Misc;
using Terraria;

namespace EquipmentEvolved.Assets.CharmsModule.Augmentations;

public abstract class AugmentationBase
{
    public abstract string LocalizedTooltip { get; }

    public virtual SpecializedPrefixType PossiblePrefixTypesToApplyOn => SpecializedPrefixType.Any;

    public abstract void EnableAugmentation(Player player);

    public abstract void DisableAugmentation(Player player);

    public virtual bool CanApply(Charm charm, Player player)
    {
        return true;
    }
}