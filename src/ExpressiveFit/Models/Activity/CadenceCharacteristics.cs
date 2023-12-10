namespace ExpressiveFit.Models.Activities;

public record CadenceCharacteristics
{
    public List<TickTuple<int>> Series { get; init; }
    public int Max { get; init; }
    public int Min { get; init; }
    public double Average { get; init; }

    public CadenceCharacteristics(List<Tick> ticks)
    {
        if (!ticks.Exists(t => t.Cadence != null))
            throw new ArgumentException("The list of ticks is missing cadence data!", nameof(ticks));

        Series = ticks.Where(t => t.Cadence is not null).Select(t => new TickTuple<int>(t.Timestamp, t.Cadence!.Value * 2)).ToList();
        Max = 2 * ticks.Max(t => t.Cadence)!.Value;
        Min = 2 * ticks.Min(t => t.Cadence)!.Value;
        Average = 2 * ticks.Average(t => t.Cadence)!.Value;
    }
}
