using Terraria;

namespace EquipmentEvolved.Assets.CharmsModule.Augmentations.ArmorAugmentations;

public class ClearingAugmentation : AugmentationBase
{
    public override string LocalizedTooltip => EquipmentEvolved.Instance
        .GetLocalization($"Augmentations.{GetType().Name}")
        .Format();

    public override void FlagEnabler(Player player)
    {
        player.GetModPlayer<AugmentationsPlayer>().ClearingAugmentation = true;
    }

    public override void FlagDisabler(Player player)
    {
        player.GetModPlayer<AugmentationsPlayer>().ClearingAugmentation = false;
    }
}