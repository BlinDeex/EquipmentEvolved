using System;
using EquipmentEvolved.Assets.Balance;

namespace EquipmentEvolved.Assets.CharmsModule.Augmentations.ArmorAugmentations;

public class PhantomAugmentation : AugmentationBase
{
    public override string LocalizedTooltip => EquipmentEvolved.Instance.GetLocalization($"Augmentations.{GetType().Name}").Format(
        PrefixBalance.PHANTOM_CLONES_COUNT_AUGMENTED, PrefixBalance.PHANTOM_AUGMENTATION_SOULBURN_RAMP_MULT);
}