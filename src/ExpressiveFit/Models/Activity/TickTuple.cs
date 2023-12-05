namespace ExpressiveFit.Models.Activities;

public record TickTuple<T>(DateTimeOffset Timestamp, T? Value);
