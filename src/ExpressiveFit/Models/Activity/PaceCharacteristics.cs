namespace ExpressiveFit.Models.Activities;

public record PaceCharacteristics
{
    public List<TickTuple<PaceValue>> Series { get; init; }
    public bool SeriesIsComplete { get; set; }
    public PaceValue Fastest { get; init; }
    public PaceValue Slowest { get; init; }
    public PaceValue Average { get; init; }
    public PaceValue MovingAverage { get; set; }

    public PaceCharacteristics(List<Tick> ticks)
    {
        if (!ticks.Exists(t => t.EnhancedSpeed != null))
            throw new ArgumentException("The list of ticks is missing speed data!", nameof(ticks));

        Series = ticks.Where(t => t.EnhancedSpeed is not null).Select(t => new TickTuple<PaceValue>(t.Timestamp, new PaceValue(t.EnhancedSpeed!.Value))).ToList();
        SeriesIsComplete = Series.Count == ticks.Count;
        var sortedBySpeed = Series.OrderBy(s => s.Value!.MetersPerSecond);
        Fastest = sortedBySpeed.Last().Value!;
        Slowest = sortedBySpeed.First().Value!;

        Average = CalculateAverage(ticks);
        MovingAverage = CalculateAverage(ticks.Where(t => t.EnhancedSpeed > 0).ToList());
    }

    private static PaceValue CalculateAverage(List<Tick> ticks)
    {
        var totalMeters = ticks.Max(t => t.Distance) - ticks.Min(t => t.Distance);
        var totalSeconds = (ticks.Max(t => t.Timestamp) - ticks.Min(t => t.Timestamp)).TotalSeconds;
        var metersPerSecond = totalMeters / totalSeconds;
        return new PaceValue(metersPerSecond ?? 0);
    }
}
