namespace ExpressiveFit.Models.Activities;

public record Tick(
    DateTimeOffset Timestamp,
    double? Latitude,
    double? Longitude,
    float? Distance,
    float? EnhancedSpeed,
    float? Altitude,
    int? HeartRate,
    int? Cadence,
    float? FractionalCadence,
    int? AccumulatedPower,
    int? Power,
    float? VerticalOscillation,
    float? VerticalRatio,
    float? StanceTimePercent,
    float? StanceTime,
    float? StanceTimeBalance,
    float? StepLength
);
