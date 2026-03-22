namespace EquipmentEvolved.Assets.Core;

public enum StatReapplicationMode
{
    /// <summary>
    /// Completely independent. Overlapping stats tick down in parallel and their values stack.
    /// </summary>
    Independent,
    
    /// <summary>
    /// Resets the timer to the maximum duration, keeping the existing value.
    /// </summary>
    RefreshDuration,
    
    /// <summary>
    /// Adds the new duration to the existing duration.
    /// </summary>
    StackDuration,
    
    /// <summary>
    /// Adds the new value to the existing value, but keeps the original duration.
    /// </summary>
    StackValue,

    /// <summary>
    /// Discards the new stat if one already exists.
    /// </summary>
    Ignore
}

public class TemporaryStat
{
    public EquipmentStat Stat;
    public float Value;
    public int TimeLeft;
    public StatSource Source;
}