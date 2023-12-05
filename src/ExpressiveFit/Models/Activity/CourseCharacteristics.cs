namespace ExpressiveFit.Models.Activities;
public record CourseCharacteristics
{
    public List<TickTuple<float>> AltitudeSeries { get; init; }
    public List<TickTuple<Coordinates>> CoordinateSeries { get; init; }
    public DistanceValue TotalDistance { get; init; }
    public float? MinAltitude { get; init; }
    public float? MaxAltitude { get; init; }
    public float TotalClimb { get; init; }
    public float TotalFall { get; init; }

    public CourseCharacteristics(List<Tick> ticks)
    {
        var baseDistance = ticks[0].Distance ?? 0;
        AltitudeSeries = ticks
            .Where(t => t.Altitude is not null)
            .Select(t => new TickTuple<float>(t.Timestamp, t.Altitude!.Value))
            .ToList();

        CoordinateSeries = ticks.Where(t => t.Longitude is not null && t.Latitude is not null)
            .Select(t => new TickTuple<Coordinates>(t.Timestamp, new Coordinates(t.Latitude!.Value, t.Longitude!.Value)))
            .ToList();

        TotalDistance = new DistanceValue((ticks.Max(t => t.Distance) ?? 0) - baseDistance);
        MaxAltitude = ticks.Max(t => t.Altitude);
        MinAltitude = ticks.Min(t => t.Altitude);
        var altitudeChange = DetermineTotalFallAndClimb(ticks);
        TotalFall = altitudeChange.Item1;
        TotalClimb = altitudeChange.Item2;
    }

    private static Tuple<float, float> DetermineTotalFallAndClimb(List<Tick> ticks)
    {
        var relevantTicks = ticks.Where(t => t.Altitude is not null).ToList();
        if (relevantTicks.Count == 0)
            return new Tuple<float, float>(0, 0);

        var previousAltitude = relevantTicks[0].Altitude!.Value;
        var totalFall = 0.0f;
        var totalClimb = 0.0f;
        foreach (var altitude in relevantTicks.Where(t => t.Altitude is not null).Select(t => t.Altitude ?? 0).Skip(1))
        {
            if (altitude < previousAltitude)
                totalFall += previousAltitude - altitude;
            else if (altitude > previousAltitude)
                totalClimb += altitude - previousAltitude;

            previousAltitude = altitude;
        }

        return new Tuple<float, float>(totalFall, totalClimb);
    }
}
