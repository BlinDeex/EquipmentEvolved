using EquipmentEvolved.Assets.Balance;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.Conduit;

public class ConduitModPlayer : ModPlayer
{
    public int ConduitPieces;

    private float manaAccumulator;
    public bool ConduitSetBonus => ConduitPieces >= 3;

    public override void ResetEffects()
    {
        ConduitPieces = 0;
    }

    public override void UpdateLifeRegen()
    {
        if (ConduitSetBonus)
        {
            Player.manaRegenDelay = 999;
            Player.manaRegen = 0;
            Player.manaRegenCount = 0;
            Player.manaRegenBonus = 0;
        }
    }

    public override void PostUpdate()
    {
        if (ConduitSetBonus && Player.statMana < Player.statManaMax2)
        {
            manaAccumulator += PrefixBalance.CONDUIT_SET_MANA_REGEN_PER_TICK;

            if (manaAccumulator >= 1f)
            {
                int manaToAdd = (int)manaAccumulator;
                Player.statMana += manaToAdd;
                manaAccumulator -= manaToAdd;

                if (Player.statMana > Player.statManaMax2) Player.statMana = Player.statManaMax2;
            }
        }
        else
            manaAccumulator = 0f;
    }
}