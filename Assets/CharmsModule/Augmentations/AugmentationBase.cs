using EquipmentEvolved.Assets.CharmsModule.Items;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using Terraria;

namespace EquipmentEvolved.Assets.CharmsModule.Augmentations;

public abstract class AugmentationBase
{
    public virtual string LocalizedTooltip => EquipmentEvolved.Instance.GetLocalization($"Augmentations.{GetType().Name}").Format();

    public virtual SpecializedPrefixType PossiblePrefixTypesToApplyOn => SpecializedPrefixType.Any;
    public virtual void EnableAugmentation(Player player)
    {
        player.GetModPlayer<StatPlayer>().AddFlag(GetType().Name);
    }

    public virtual bool CanApply(Charm charm, Player player) => true;
}