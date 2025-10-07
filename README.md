# ExpressiveFit
A C# library with expressive models to interact with the content of .FIT-files.

### Installation
Simply clone this repository and reference the project `ExpressiveFit.csproj`

### Usage

Read a .FIT-file
```cs
// From file path
Activity activity = ReadUtils.ReadFile("my_fit_file.fit");

// From file stream
FileStream stream = new FileStream("my_fit_file.fit", FileMode.Open);
Activity activity = ReadUtils.ReadFile(stream);

// From directory
List<Activity> activities = ReadUtils.ReadDirectory("mydir/");
```

Interact with expressive activity models
```cs
var activity = ReadUtils.ReadFile("my_fit_file.fit");

var averageSpeedKilometersPerHour = activity.PaceCharacteristics.Average.KilometersPerHour;
var maxHeartRate = activity.HeartRateCharacteristics.Max;
var averageHeartRateLap2 = activity.Laps[1].HeartRateCharacteristics.Average;
var heartRateSeries = activity.HeartReateCharacteristics.Series;

```

Simply check the model defintions in `/src/Models/Activity/` to see all available data fields.
