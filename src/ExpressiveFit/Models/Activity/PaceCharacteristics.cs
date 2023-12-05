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
        if (!ticks.Any(t => t.EnhancedSpeed != null))
            throw new ArgumentException("The list of ticks is missing speed data!", nameof(ticks));

        Series = ticks.Where(t => t.EnhancedSpeed is not null).Select(t => new TickTuple<PaceValue>(t.Timestamp, new PaceValue(t.EnhancedSpeed!.Value))).ToList();
        SeriesIsComplete = Series.Count == ticks.Count;
        var sortedBySpeed = Series.OrderBy(s => s.Value!.MetersPerSecond);
        Fastest = sortedBySpeed.Last().Value!;
        Slowest = sortedBySpeed.First().Value!;

        var average = Series.Average(t => t.Value!.MetersPerSecond);
        Average = new PaceValue(average);

        var averageMoving = Series.Where(t => t.Value!.MetersPerSecond > 0).Average(t => t.Value!.MetersPerSecond);
        MovingAverage = new PaceValue(averageMoving);
    }
}
