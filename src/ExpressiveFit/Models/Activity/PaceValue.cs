namespace ExpressiveFit.Models.Activities;

public record PaceValue
{
    private readonly double _metersPerSecond;
    public double MetersPerSecond => _metersPerSecond;
    public double KilometersPerHour => _metersPerSecond * 3.6;
    public double MilesPerHour => KilometersPerHour / 1.609344;
    public double MinutesPerKilometer => KilometersPerHour > 0 ? 60 / KilometersPerHour : double.PositiveInfinity;
    public double MinutesPerMile => MinutesPerKilometer * 1.609344;
    public PaceValue(double metersPerSecond)
    {
        _metersPerSecond = metersPerSecond;
    }
}