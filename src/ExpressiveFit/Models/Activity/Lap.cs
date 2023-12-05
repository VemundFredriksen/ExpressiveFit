namespace ExpressiveFit.Models.Activities;

public class Lap
{
    public List<Tick> Ticks { get; init; }
    public TimeCharacterstics TimeCharacterstics { get; init; }
    public CourseCharacteristics CourseCharacteristics { get; init; }
    public HeartRateCharacteristics? HeartRateCharacteristics { get; init; }
    public CadenceCharacteristics? CadenceCharacteristics { get; init; }
    public PowerCharacteristics? PowerCharacteristics { get; init; }
    public PaceCharacteristics? PaceCharacteristics { get; set; }

    public Lap(List<Tick> ticks)
    {
        if (ticks.Count == 0)
            throw new ArgumentException("Tick list cannot be empty.", nameof(ticks));

        Ticks = [.. ticks.OrderBy(t => t.Timestamp)];

        TimeCharacterstics = new TimeCharacterstics(ticks);
        CourseCharacteristics = new CourseCharacteristics(ticks);

        if (ticks.Any(t => t.HeartRate != null))
            HeartRateCharacteristics = new HeartRateCharacteristics(ticks);

        if (ticks.Any(t => t.Cadence != null))
            CadenceCharacteristics = new CadenceCharacteristics(ticks);

        if (ticks.Any(t => t.Power != null))
            PowerCharacteristics = new PowerCharacteristics(ticks);

        if (ticks.Any(t => t.EnhancedSpeed != null))
            PaceCharacteristics = new PaceCharacteristics(ticks);
    }
}
