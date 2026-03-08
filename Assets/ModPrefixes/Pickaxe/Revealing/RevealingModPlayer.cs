using EquipmentEvolved.Assets.Balance;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Pickaxe.Revealing;

public class RevealingModPlayer : ModPlayer
{
    public int RevealingTicks { get; private set; }

    public void SetRevealing()
    {
        RevealingTicks = PrefixBalance.REVEALING_TICKS;
    }

    public override void PostUpdateEquips()
    {
        if (RevealingTicks > 0) RevealingTicks--;
    }
}