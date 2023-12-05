namespace ExpressiveFit.Models.Activities;

public record DistanceValue
{
    private readonly double _meters;
    public double Kilometers => _meters / 1000;
    public double Miles => _meters / 1609.344;
    public double AU => _meters / 149_597_870_700;
    public DistanceValue(float meters)
    {
        _meters = meters;
    }
}
