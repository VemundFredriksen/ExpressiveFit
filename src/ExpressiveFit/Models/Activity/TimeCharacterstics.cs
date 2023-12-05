namespace ExpressiveFit.Models.Activities;

public record TimeCharacterstics
{
    public List<DateTimeOffset> Series { get; init; }
    public DateTimeOffset Start { get; init; }
    public DateTimeOffset End { get; init; }
    public TimeSpan Duration { get; init; }

    public TimeCharacterstics(List<Tick> ticks)
    {
        Series = ticks.Select(t => t.Timestamp).ToList();
        Start = ticks.OrderBy(t => t.Timestamp).First().Timestamp;
        End = ticks.OrderBy(t => t.Timestamp).Last().Timestamp;
        Duration = End - Start;
    }
}
