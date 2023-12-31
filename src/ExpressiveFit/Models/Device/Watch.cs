﻿namespace ExpressiveFit.Models.Devices;

public record Watch(ModelType Model) : GarminDevice(Model)
{
    public static List<ModelType> WatchModels =>
    [
        ModelType.Fenix,
        ModelType.Fenix2,
        ModelType.Fenix3,
        ModelType.Fenix3China,
        ModelType.Fenix3Chronos,
        ModelType.Fenix3ChronosAsia,
        ModelType.Fenix3HrChn,
        ModelType.Fenix3HrJpn,
        ModelType.Fenix3HrKor,
        ModelType.Fenix3HrSea,
        ModelType.Fenix3HrTwn,
        ModelType.Fenix5,
        ModelType.Fenix5Asia,
        ModelType.Fenix5Plus,
        ModelType.Fenix5s,
        ModelType.Fenix5sAsia,
        ModelType.Fenix5sPlus,
        ModelType.Fenix5sPlusApac,
        ModelType.Fenix5x,
        ModelType.Fenix5xAsia,
        ModelType.Fenix5xPlus,
        ModelType.Fenix5xPlusApac,
        ModelType.Fenix6,
        ModelType.Fenix6Asia,
        ModelType.Fenix6s,
        ModelType.Fenix6sAsia,
        ModelType.Fenix6Sport,
        ModelType.Fenix6SportAsia,
        ModelType.Fenix6sSport,
        ModelType.Fenix6sSportAsia,
        ModelType.Fenix6x,
        ModelType.Fenix6xAsia,
        ModelType.Fenix7,
        ModelType.Fenix7Apac,
        ModelType.Fenix7ProSolar,
        ModelType.Fenix7s,
        ModelType.Fenix7sApac,
        ModelType.Fenix7sProSolar,
        ModelType.Fenix7x,
        ModelType.Fenix7xApac,
        ModelType.Fenix7xProSolar,

    ];
}