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
    const string FIELD_TIMESTAMP = "Timestamp";
    const string FIELD_LATITUDE = "PositionLat";
    const string FIELD_LONGITUDE = "PositionLong";
    const string FIELD_DISTANCE = "Distance";
    const string FIELD_ENHANCED_SPEED = "EnhancedSpeed";
    const string FIELD_ENHANCED_ALTITUDE = "EnhancedAltitude";
    const string FIELD_HEART_RATE = "HeartRate";
    const string FIELD_CADENCE = "Cadence";
    const string FIELD_FRACTIONAL_CADENCE = "FractionalCadence";
    const string FIELD_ACCUMULATED_POWER = "AccumulatedPower";
    const string FIELD_POWER = "Power";
    const string FIELD_VERTICAL_OSCILLATION = "VerticalOscillation";
    const string FIELD_STANCE_TIME_PERCENT = "StanceTimePercent";
    const string FIELD_STANCE_TIME = "StanceTime";
    const string FIELC_STANCE_TIME_BALANCE = "StanceTimeBalance";
    const string FIELD_ACTIVITY_TYPE = "ActivityType";
    const string FIELD_VERTICAL_RATIO = "VerticalRatio";
    const string FIELD_STEP_LENGTH = "StepLength";

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
        _laps = [];
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
            if (field.Name == FIELD_TIMESTAMP)
                timestamp = ToDateTimeOffset((uint)field.GetValue());
            else if ((field.Name == FIELD_LATITUDE))
                latitude = SemicirclesToDegrees((int)field.GetValue());
            else if ((field.Name == FIELD_LONGITUDE))
                longitude = SemicirclesToDegrees((int)field.GetValue());
            else if ((field.Name == FIELD_DISTANCE))
                distance = GetNullableField<float>(field);
            else if ((field.Name == FIELD_ENHANCED_SPEED))
                enhancedSpeed = GetNullableField<float>(field);
            else if ((field.Name == FIELD_ENHANCED_ALTITUDE))
                enhancedAltitude = GetNullableField<float>(field);
            else if ((field.Name == FIELD_HEART_RATE))
                heartRate = GetNullableField<byte>(field);
            else if ((field.Name == FIELD_CADENCE))
                cadence = GetNullableField<byte>(field);
            else if ((field.Name == FIELD_FRACTIONAL_CADENCE))
                fractionalCadence = GetNullableField<float>(field);
            else if ((field.Name == FIELD_ACCUMULATED_POWER))
                accumulatedPower = GetNullableField<int>(field);
            else if ((field.Name == FIELD_POWER))
                power = GetNullableField<int>(field) ?? GetNullableField<ushort>(field);
            else if ((field.Name == FIELD_VERTICAL_OSCILLATION))
                verticalOscillation = GetNullableField<float>(field);
            else if ((field.Name == FIELD_VERTICAL_RATIO))
                verticalRatio = GetNullableField<float>(field);
            else if ((field.Name == FIELD_STANCE_TIME_PERCENT))
                stanceTimePercent = GetNullableField<float>(field);
            else if ((field.Name == FIELD_STANCE_TIME))
                stanceTime = GetNullableField<float>(field);
            else if ((field.Name == FIELC_STANCE_TIME_BALANCE))
                stanceTimeBalance = GetNullableField<float>(field);
            else if ((field.Name == FIELD_STEP_LENGTH))
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

internal record FitDeviceInfo(int Manufacturer, int? Model);