using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.CharmsModule.Augmentations;

public class AugmentationsPlayer : ModPlayer
{
    public bool PhantomAugmentation { get; set; }
    public bool ClearingAugmentation { get; set; }
    public bool InvertedAugmentation { get; set; }
    public bool CursedAugmentation { get; set; }

    public bool OmniAugmentation { get; set; }

    public override void ResetEffects()
    {
        PhantomAugmentation = false;
        ClearingAugmentation = false;
        InvertedAugmentation = false;
        CursedAugmentation = false;
        OmniAugmentation = false;
    }
}