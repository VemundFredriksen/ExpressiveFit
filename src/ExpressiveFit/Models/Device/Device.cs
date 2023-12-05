namespace ExpressiveFit.Models.Devices;

public record Device(ManufacturerType Manufacturer)
{
    public static Device FromManufacturerType(ManufacturerType manufacturerType, ModelType? modelType = null)
    {
        if(manufacturerType == ManufacturerType.Garmin && modelType != null)
        {
            return GarminDevice.FromModelType(modelType.Value);
        }

        return new Device(manufacturerType);
    }
}
