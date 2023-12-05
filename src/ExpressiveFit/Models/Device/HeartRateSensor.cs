namespace ExpressiveFit.Models.Devices;

public record HeartRateSensor(ModelType Model) : GarminDevice(Model)
{
    public static List<ModelType> HeartRateSensorModels =>
    [
        ModelType.Hrm1,
        ModelType.Hrm2ss,
        ModelType.Hrm3ss,
        ModelType.Hrm4Run,
        ModelType.Hrm4RunSingleByteProductId,
        ModelType.HrmDual,
        ModelType.HrmPro,
        ModelType.HrmProPlus,
        ModelType.HrmRun,
        ModelType.HrmRunSingleByteProductId,
        ModelType.HrmTri,
        ModelType.HrmTriSingleByteProductId,
    ];
}