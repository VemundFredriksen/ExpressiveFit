using Dynastream.Fit;
using ExpressiveFit.Models.Activities;
using ExpressiveFit.Models.Devices;
using Activity = ExpressiveFit.Models.Activities.Activity;

namespace ExpressiveFit.Readers;

public interface IFitReader
{
    Activity ReadFitFile(FileStream fileStream);
}

public class FitReader : IFitReader
{
    private Decode _decoder;
    private List<Tick> _ticks = [];
    private HashSet<FitDeviceInfo> _fitDeviceInfos = [];
    private List<DateTimeOffset> _laps = [];

    public FitReader()
    {
        _decoder = new Decode();
    }

    public Activity ReadFitFile(FileStream fileStream)
    {
        ClearDecoder();

        if(!_decoder.IsFIT(fileStream))
            throw new IOException("The file was not recognized as a .FIT-file!");

        if (!_decoder.CheckIntegrity(fileStream))
            throw new IOException("Integrity of the file was not valid!");

        fileStream.Seek(0, SeekOrigin.Begin);
        _decoder.Read(fileStream);

        fileStream.Close();

        var devices = _fitDeviceInfos.Select(f => Device.FromManufacturerType((ManufacturerType)f.Manufacturer, (ModelType?) f.Model ?? ModelType.Unknown)).ToList();

        return new Activity(devices, _ticks, _laps);
    }

    private void ClearDecoder()
    {
        _decoder = new Decode();
        _decoder.MesgEvent += OnMesgEvent;
        _ticks = [];
        _fitDeviceInfos = [];
    }

    private void OnMesgEvent(object sender, MesgEventArgs e)
    {
        if(e.mesg.Name == "Record")
        {
            var tick = ReadRecord(e.mesg);
            _ticks.Add(tick);
        }
        else if(e.mesg.Name == "DeviceInfo")
        {
            if (!e.mesg.Fields.Any(f => f.Name == "Manufacturer"))
                return;

            var deviceInfo = ReadDevice(e.mesg);
            if (deviceInfo is null)
                return;

            _fitDeviceInfos.Add(deviceInfo);
        }
        else if(e.mesg.Name == "Lap")
        {
            var laptime = ToDateTimeOffset((uint)e.mesg.Fields.Single(f => f.Name == "Timestamp").GetValue());
            _laps.Add(laptime);
        }
    }

    private static FitDeviceInfo? ReadDevice(Mesg message)
    {
        if (!message.Fields.Any(f => f.Name == "Manufacturer"))
        {
            return null;
        }

        var manufacturer = (ushort) message.Fields.Single(f => f.Name == "Manufacturer").GetValue();
        ushort? model = (ushort?) message.Fields.Single(f => f.Name == "Product")?.GetValue();

        return new FitDeviceInfo(manufacturer, model);
    }

    private static Tick ReadRecord(Mesg message)
    {
        DateTimeOffset? timestamp = null;
        double? latitude = null;
        double? longitude = null;
        float? distance = null;
        float? enhancedSpeed = null;
        float? enhancedAltitude = null;
        int? heartRate = null;
        int? cadence = null;
        float? fractionalCadence = null;
        int? accumulatedPower = null;
        int? power = null;
        float? verticalOscillation = null;
        float? verticalRatio = null;
        float? stanceTimePercent = null;
        float? stanceTime = null;
        float? stanceTimeBalance = null;
        float? stepLengt = null;

        if (message.Name != "Record")
            throw new InvalidOperationException();

        foreach (var field in message.Fields)
        {
            if (field.Name == FitFieldNames.Timestamp)
                timestamp = ToDateTimeOffset((uint)field.GetValue());
            else if ((field.Name == FitFieldNames.Latitude))
                latitude = SemicirclesToDegrees((int)field.GetValue());
            else if ((field.Name == FitFieldNames.Longitude))
                longitude = SemicirclesToDegrees((int)field.GetValue());
            else if ((field.Name == FitFieldNames.Distance))
                distance = GetNullableField<float>(field);
            else if ((field.Name == FitFieldNames.EnhancedSpeed))
                enhancedSpeed = GetNullableField<float>(field);
            else if ((field.Name == FitFieldNames.EnhancedAltitude))
                enhancedAltitude = GetNullableField<float>(field);
            else if ((field.Name == FitFieldNames.HeartRate))
                heartRate = GetNullableField<byte>(field);
            else if ((field.Name == FitFieldNames.Cadence))
                cadence = GetNullableField<byte>(field);
            else if ((field.Name == FitFieldNames.FractionalCadence))
                fractionalCadence = GetNullableField<float>(field);
            else if ((field.Name == FitFieldNames.AccumulatedPower))
                accumulatedPower = GetNullableField<int>(field);
            else if ((field.Name == FitFieldNames.Power))
                power = GetNullableField<int>(field) ?? GetNullableField<ushort>(field);
            else if ((field.Name == FitFieldNames.VerticalOscillation))
                verticalOscillation = GetNullableField<float>(field);
            else if ((field.Name == FitFieldNames.VerticalRatio))
                verticalRatio = GetNullableField<float>(field);
            else if ((field.Name == FitFieldNames.StanceTimePercent))
                stanceTimePercent = GetNullableField<float>(field);
            else if ((field.Name == FitFieldNames.StanceTime))
                stanceTime = GetNullableField<float>(field);
            else if ((field.Name == FitFieldNames.StanceTimeBalance))
                stanceTimeBalance = GetNullableField<float>(field);
            else if ((field.Name == FitFieldNames.StepLength))
                stepLengt = GetNullableField<float>(field);
        }

        if (timestamp == null)
            throw new FormatException("Timestamp is required for every tick!");

        return new Tick(
            timestamp.Value, 
            latitude, 
            longitude, 
            distance, 
            enhancedSpeed, 
            enhancedAltitude, 
            heartRate, 
            cadence,
            fractionalCadence,
            accumulatedPower, 
            power,
            verticalOscillation,
            verticalRatio, 
            stanceTimePercent, 
            stanceTime, 
            stanceTimeBalance, 
            stepLengt
        );
    }

    private static T? GetNullableField<T>(Field field)
        where T : struct
    {
        if (field.GetValue() is T value)
            return value;

        return null;
    }

    private static DateTimeOffset ToDateTimeOffset(uint fitTime)
    {
        DateTimeOffset fitBaseTime = new(1989, 12, 31, 0, 0, 0, TimeSpan.Zero);
        DateTimeOffset timestamp = fitBaseTime.AddSeconds(fitTime);

        return timestamp;
    }

    private static double SemicirclesToDegrees(int semicircles)
    {
        return semicircles * (180.0 / Math.Pow(2, 31));
    }
}

internal static class FitFieldNames
{
    public static string Timestamp = "Timestamp";
    public static string Latitude = "PositionLat";
    public static string Longitude = "PositionLong";
    public static string Distance = "Distance";
    public static string EnhancedSpeed = "EnhancedSpeed";
    public static string EnhancedAltitude = "EnhancedAltitude";
    public static string HeartRate = "HeartRate";
    public static string Cadence = "Cadence";
    public static string FractionalCadence = "FractionalCadence";
    public static string AccumulatedPower = "AccumulatedPower";
    public static string Power = "Power";
    public static string VerticalOscillation = "VerticalOscillation";
    public static string StanceTimePercent = "StanceTimePercent";
    public static string StanceTime = "StanceTime";
    public static string StanceTimeBalance = "StanceTimeBalance";
    public static string ActivityType = "ActivityType";
    public static string VerticalRatio = "VerticalRatio";
    public static string StepLength = "StepLength";
}

internal record FitDeviceInfo(int Manufacturer, int? Model);