namespace ExpressiveFit.Models.Activities;

public record PowerCharacteristics
{
    public List<TickTuple<int>> Series { get; init; }
    public bool SeriesIsComplete { get; set; }
    public PowerValue Max { get; init; }
    public PowerValue Min { get; init; }
    public PowerValue Average { get; init; }
    public WorkValue TotalPower { get; init; }

    public PowerCharacteristics(List<Tick> ticks)
    {
        if (!ticks.Exists(t => t.Power != null))
            throw new ArgumentException("The list of ticks is missing power data!", nameof(ticks));

        Series = ticks.Where(t => t.Power is not null).Select(t => new TickTuple<int>(t.Timestamp, t.Power!.Value)).ToList();
        SeriesIsComplete = Series.Count == ticks.Count;
        Max = new PowerValue(ticks.Max(t => t.Power)!.Value);
        Min = new PowerValue(ticks.Min(t => t.Power)!.Value);
        Average = new PowerValue(ticks.Average(t => t.Power)!.Value);
        TotalPower = CalculateTotalWattHours(ticks);
    }

    private WorkValue CalculateTotalWattHours(List<Tick> ticks)
    {
        var duration = ticks.Max(t => t.Timestamp) - ticks.Min(t => t.Timestamp);
        var averageWatts = ticks.Average(t => t.Power) ?? 0;
        return new(averageWatts * duration.TotalHours);
    }
}

public record PowerValue
{
    private readonly double _watts;
    public double Watts => _watts;
    public double HorsePowers => _watts * 0.00134102209;

    public PowerValue(double watts)
    {
        _watts = watts;
    }
}

public record WorkValue
{
    private readonly double _wattHours;
    public double WattHours => _wattHours;
    public double Joules => _wattHours * 3600;
    public double KiloJoules => Joules / 1000;

    public WorkValue(double wattHours)
    {
        _wattHours = wattHours;
    }
}