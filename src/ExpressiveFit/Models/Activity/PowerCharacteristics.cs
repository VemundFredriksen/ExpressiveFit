namespace ExpressiveFit.Models.Activities;

public record PowerCharacteristics
{
    public List<TickTuple<int>> Series { get; init; }
    public bool SeriesIsComplete { get; set; }
    public int Max { get; init; }
    public int Min { get; init; }
    public double Average { get; init; }
    public int TotalPower { get; init; }

    public PowerCharacteristics(List<Tick> ticks)
    {
        if (!ticks.Any(t => t.Power != null))
            throw new ArgumentException("The list of ticks is missing power data!", nameof(ticks));

        Series = ticks.Where(t => t.Power is not null).Select(t => new TickTuple<int>(t.Timestamp, t.Power!.Value)).ToList();
        SeriesIsComplete = Series.Count == ticks.Count;
        Max = ticks.Max(t => t.Power)!.Value;
        Min = ticks.Min(t => t.Power)!.Value;
        Average = ticks.Average(t => t.Power)!.Value;
        TotalPower = ticks.Sum(t => t.Power)!.Value;
    }
}
