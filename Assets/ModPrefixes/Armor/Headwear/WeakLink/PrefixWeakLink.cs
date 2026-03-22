using System.Collections.Generic;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Core;
using EquipmentEvolved.Assets.ModPrefixes.Core;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Headwear.WeakLink;

public class PrefixWeakLink : BaseEvolvedPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override float ReforgeMultiplier => PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Headwear;
    
    protected override void OnSetStaticDefaults()
    {
        
        
        // Subscribe our custom logic to the central tooltip manager!
        DefenseTooltipGlobalItem.DefenseModifiers.Add((item, player) =>
        {
            int netModifier = 0;
            if (item.prefix == Type)
            {
                netModifier -= (int)(item.defense * (1f - PrefixBalance.WEAK_LINK_THIS_DEFENSE_MULT));
            }
            
            if (item.bodySlot != -1 || item.legSlot != -1)
            {
                if (player.armor[0] != null && !player.armor[0].IsAir && player.armor[0].prefix == Type)
                {
                    netModifier += (int)(item.defense * (PrefixBalance.WEAK_LINK_OTHER_DEFENSE_MULT - 1f));
                }
            }

            return netModifier;
        });
    }

    protected override IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        int selfLoss = (int)((1f - PrefixBalance.WEAK_LINK_THIS_DEFENSE_MULT) * 100);
        int otherGain = (int)((PrefixBalance.WEAK_LINK_OTHER_DEFENSE_MULT - 1f) * 100);

        yield return new TooltipLine(Mod, "WeakLinkDescription", Description.Format(selfLoss, otherGain))
        {
            IsModifier = true
        };
    }
}