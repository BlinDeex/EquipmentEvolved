using System.Collections.Generic;
using EquipmentEvolved.Assets.CharmsModule.Augmentations;
using EquipmentEvolved.Assets.Core;

namespace EquipmentEvolved.Assets.CharmsModule.Data;

public class CharmDataSnapshot
{
    public List<AugmentationBase> Augmentations;
    public string CharmName;
    public CharmRarity Rarity;
    public List<CharmRoll> Stats;

    public CharmDataSnapshot Clone()
    {
        return new CharmDataSnapshot
        {
            Rarity = Rarity,
            Augmentations = Augmentations,
            Stats = [..Stats],
            CharmName = CharmName
        };
    }
}