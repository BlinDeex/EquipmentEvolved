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
            PlayerStat.Crit => false,
            _ => true
        };
    }

    public float GetStrength()
    {
        float displayStrength = strength;
        if (stat == PlayerStat.Regen) // regen is applied over 2 secs
        {
            displayStrength /= 2f;
        }
        
        return MathF.Round(displayStrength * (IsPercentage() ? 100 : 1), 2);
    }
}