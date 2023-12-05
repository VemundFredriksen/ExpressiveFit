using ExpressiveFit.Models.Devices;

namespace ExpressiveFit.Models.Activities;

public class Activity
{
    public ActivityType Type { get; init; }
    public List<Device> Devices { get; init; }
    public List<Tick> Ticks { get; init; }
    public List<Lap> Laps { get; set; }
    public TimeCharacterstics TimeCharacterstics { get; init; }
    public CourseCharacteristics CourseCharacteristics { get; init; }
    public HeartRateCharacteristics? HeartRateCharacteristics { get; init; }
    public CadenceCharacteristics? CadenceCharacteristics { get; init; }
    public PowerCharacteristics? PowerCharacteristics { get; init; }
    public PaceCharacteristics? PaceCharacteristics { get; set; }


    public Activity(List<Device> devices, List<Tick> ticks, List<DateTimeOffset> laps)
    {
        if (ticks.Count == 0)
            throw new ArgumentException("Tick list cannot be empty.", nameof(ticks));

        Devices = devices;
        Ticks = [.. ticks.OrderBy(t => t.Timestamp)];

        TimeCharacterstics = new TimeCharacterstics(ticks);
        CourseCharacteristics = new CourseCharacteristics(ticks);

        if (ticks.Any(t => t.HeartRate != null))
            HeartRateCharacteristics = new HeartRateCharacteristics(ticks);

        if (ticks.Any(t => t.Cadence != null))
            CadenceCharacteristics = new CadenceCharacteristics(ticks);

        if (ticks.Any(t => t.Power != null))
            PowerCharacteristics = new PowerCharacteristics(ticks);

        if(ticks.Any(t => t.EnhancedSpeed != null))
            PaceCharacteristics = new PaceCharacteristics(ticks);

        Laps = [];
        var remainingTicks = Ticks;
        foreach (var lap in laps.OrderBy(l => l))
        {
            Laps.Add(new Lap(remainingTicks.Where(t => t.Timestamp < lap).ToList()));
            remainingTicks = remainingTicks.Where(t => t.Timestamp >= lap).ToList();
        }
    }
}