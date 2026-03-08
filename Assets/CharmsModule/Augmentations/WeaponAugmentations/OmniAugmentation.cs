using EquipmentEvolved.Assets.CharmsModule.Data;
using EquipmentEvolved.Assets.CharmsModule.Items;
using Terraria;

namespace EquipmentEvolved.Assets.CharmsModule.Augmentations.WeaponAugmentations;

public class OmniAugmentation : AugmentationBase
{
    public override string LocalizedTooltip =>
        EquipmentEvolved.Instance.GetLocalization($"Augmentations.{GetType().Name}").Value;

    public override void EnableAugmentation(Player player)
    {
        player.GetModPlayer<AugmentationsPlayer>().OmniAugmentation = true;
    }

    public override void DisableAugmentation(Player player)
    {
        player.GetModPlayer<AugmentationsPlayer>().OmniAugmentation = false;
    }

    public override bool CanApply(Charm charm, Player player)
    {
        return charm.CharmType == CharmType.Circle && charm.CharmRarity == CharmRarity.Exalted;
    }
}