using EquipmentEvolved.Assets.CharmsModule.Data;
using EquipmentEvolved.Assets.CharmsModule.Items;
using Terraria;

namespace EquipmentEvolved.Assets.CharmsModule.Augmentations.WeaponAugmentations;

public class OmniFailedAugmentation : AugmentationBase
{
    public override string LocalizedTooltip =>
        EquipmentEvolved.Instance.GetLocalization($"Augmentations.{GetType().Name}").Value;

    public override void EnableAugmentation(Player player)
    {
    }

    public override void DisableAugmentation(Player player)
    {
    }

    public override bool CanApply(Charm charm, Player player)
    {
        return charm.CharmType == CharmType.Circle && charm.CharmRarity == CharmRarity.Exalted;
    }
}