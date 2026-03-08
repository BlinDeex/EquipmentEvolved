using System;
using EquipmentEvolved.Assets.Core;

namespace EquipmentEvolved.Assets.CharmsModule.Data;

public class CharmRoll(PlayerStat stat, float strength)
{
    public PlayerStat Stat => stat;
    public float RawStrength => strength;

    private bool IsPercentage()
    {
        return Stat switch
        {
            // Add any future flat stats to this list
            PlayerStat.Iframes => false,
            PlayerStat.Regen => false,
            PlayerStat.LifeSteal => false,
            PlayerStat.WingTime => false,
            _ => true
        };
    }

    public float GetStrength()
    {
        return MathF.Round(strength * (IsPercentage() ? 100 : 1), 2);
    }
}