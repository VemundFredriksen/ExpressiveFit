namespace ExpressiveFit.Models.Activities;

public record HeartRateCharacteristics
{
    public List<TickTuple<int>> Series { get; init; }
    public bool SeriesIsComplete { get; set; }
    public int Max { get; init; }
    public int Min { get; init; }
    public double Average { get; init; }
    public int TotalBeats { get; init; }

    public HeartRateCharacteristics(List<Tick> ticks)
    {
        if (!ticks.Any(t => t.HeartRate != null))
            throw new ArgumentException("The list of ticks is missing heart rate data!", nameof(ticks));

        Series = ticks.Where(t => t.HeartRate is not null).Select(t => new TickTuple<int>(t.Timestamp, t.HeartRate!.Value)).ToList();
        SeriesIsComplete = Series.Count == ticks.Count;
        Max = ticks.Max(t => t.HeartRate)!.Value;
        Min = ticks.Min(t => t.HeartRate)!.Value;
        Average = ticks.Average(t => t.HeartRate)!.Value;
        TotalBeats = (int) Math.Floor(Average * (Series.Last().Timestamp - Series.First().Timestamp).TotalMinutes);
    }
}
