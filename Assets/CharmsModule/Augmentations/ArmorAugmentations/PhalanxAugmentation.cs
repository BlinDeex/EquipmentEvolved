using System;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Misc;

namespace EquipmentEvolved.Assets.CharmsModule.Augmentations.ArmorAugmentations;

public class PhalanxAugmentation : AugmentationBase
{
    public override string LocalizedTooltip => EquipmentEvolved.Instance.GetLocalization($"Augmentations.{GetType().Name}").Format(
        Math.Round(PrefixBalance.PHALANX_AUGMENTATION_OVERCLOCK_TICKS / 60f, 1));
}