using System;
using EquipmentEvolved.Assets.Balance;

namespace EquipmentEvolved.Assets.CharmsModule.Augmentations.ArmorAugmentations;

public class CursedAugmentation : AugmentationBase
{
    public override string LocalizedTooltip => EquipmentEvolved.Instance.GetLocalization($"Augmentations.{GetType().Name}").Format(
        Math.Round(PrefixBalance.CURSED_IGNORED_DAMAGE_PERCENT_THRESHOLD * 100, 2), Math.Round(PrefixBalance.AUGMENTATION_CURSED_IGNORED_DAMAGE_PERCENT_THRESHOLD * 100, 2),
        Math.Round(PrefixBalance.CURSED_DAMAGE_TAKEN_PERCENT * 100, 2), Math.Round(PrefixBalance.AUGMENTATION_CURSED_DAMAGE_TAKEN_PERCENT * 100, 2));
}