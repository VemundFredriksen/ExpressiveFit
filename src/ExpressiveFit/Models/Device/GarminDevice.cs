namespace ExpressiveFit.Models.Devices;

public record GarminDevice(ModelType Product) : Device(ManufacturerType.Garmin)
{
    public static GarminDevice FromModelType(ModelType modelType)
    {
        if (Watch.WatchModels.Contains(modelType))
        {
            return new Watch(modelType);
        }
        else if (HeartRateSensor.HeartRateSensorModels.Contains(modelType))
        {
            return new HeartRateSensor(modelType);
        }

        return new GarminDevice(modelType);
    }
}
