using EquipmentEvolved.Assets.CharmsModule.Data;
using EquipmentEvolved.Assets.CharmsModule.Items;
using Terraria;

namespace EquipmentEvolved.Assets.CharmsModule.Augmentations.WeaponAugmentations;

public class OmniAugmentation : AugmentationBase
{
    public override bool CanApply(Charm charm, Player player)
    {
        return charm.CharmType == CharmType.Circle && charm.CharmRarity == CharmRarity.Exalted;
    }
}