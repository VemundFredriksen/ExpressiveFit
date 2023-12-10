using ExpressiveFit.Utils;
using FluentAssertions;
using FluentAssertions.Execution;

namespace ScenarioTests;

public class Fenix7XRunningIndoorsScenario
{
    [Fact]
    public void ReadFile_Fenix7XIndoorsRunning_ValidateTimeCharacteristics()
    {
        // Arrange
        var filePath = "./Samples/fenix7x_running_indoor.fit";
        
        // Act
        var activity = ReadUtils.ReadFile(filePath);

        // Assert
        var timeCharacteristics = activity.TimeCharacterstics;
        using (new AssertionScope())
        {
            timeCharacteristics.Start.Should().BeCloseTo(new DateTimeOffset(2023, 12, 5, 14, 50, 0, TimeSpan.Zero), TimeSpan.FromMinutes(1));
            timeCharacteristics.End.Should().BeCloseTo(new DateTimeOffset(2023, 12, 5, 15, 52, 0, TimeSpan.Zero), TimeSpan.FromMinutes(1));
            timeCharacteristics.Duration.Should().BeCloseTo(new TimeSpan(1, 02, 01), TimeSpan.FromSeconds(1));
        }
    }

    [Fact]
    public void ReadFile_Fenix7XIndoorsRunning_ValidatePaceCharacteristics()
    {
        // Arrange
        var filePath = "./Samples/fenix7x_running_indoor.fit";

        // Act
        var activity = ReadUtils.ReadFile(filePath);

        // Assert
        var paceCharacteristics = activity.PaceCharacteristics;
        using (new AssertionScope())
        {
            paceCharacteristics!.Slowest.MinutesPerKilometer.Should().Be(double.PositiveInfinity);   
            paceCharacteristics.Fastest.MinutesPerKilometer.Should().BeApproximately(4.183, 0.01);
            paceCharacteristics.Average.MinutesPerKilometer.Should().BeApproximately(5.883, 0.01);
            paceCharacteristics.MovingAverage.MinutesPerKilometer.Should().BeApproximately(5.083, 0.01);
        }
    }

    [Fact]
    public void ReadFile_Fenix7XIndoorsRunning_ValidateHeartRateCharacterstics()
    {
        // Arrange
        var filePath = "./Samples/fenix7x_running_indoor.fit";

        // Act
        var activity = ReadUtils.ReadFile(filePath);

        // Assert
        var heartRateCharacteristics = activity.HeartRateCharacteristics;
        using (new AssertionScope())
        {
            heartRateCharacteristics!.Min.Should().Be(62);
            heartRateCharacteristics.Max.Should().Be(181);
            heartRateCharacteristics.Average.Should().BeApproximately(148, 1);
            heartRateCharacteristics.TotalBeats.Should().BeCloseTo(9155, 1);
        }
    }

    [Fact]
    public void ReadFile_Fenix7XIndoorsRunning_ValidatePowerCharacterstics()
    {
        // Arrange
        var filePath = "./Samples/fenix7x_running_indoor.fit";

        // Act
        var activity = ReadUtils.ReadFile(filePath);

        // Assert
        var powerCharacteristics = activity.PowerCharacteristics;
        using (new AssertionScope())
        {
            powerCharacteristics!.Max.Watts.Should().BeApproximately(490, 1);
            powerCharacteristics.Min.Watts.Should().BeApproximately(0, 1);
            powerCharacteristics.Average.Watts.Should().BeApproximately(319, 1);
            powerCharacteristics.TotalPower.WattHours.Should().BeApproximately(329.81, 1);
        }
    }

    [Fact]
    public void ReadFile_Fenix7XIndoorsRunning_ValidateCadenceCharacterstics()
    {
        // Arrange
        var filePath = "./Samples/fenix7x_running_indoor.fit";

        // Act
        var activity = ReadUtils.ReadFile(filePath);

        // Assert
        var cadenceCharacteristics = activity.CadenceCharacteristics;
        using (new AssertionScope())
        {
            cadenceCharacteristics!.Min.Should().Be(0);
            cadenceCharacteristics.Max.Should().Be(360);
            cadenceCharacteristics.Average.Should().BeApproximately(142, 1);
        }
    }

    [Fact]
    public void ReadFile_Fenix7XIndoorsRunning_ValidateCourseCharacterstics()
    {
        // Arrange
        var filePath = "./Samples/fenix7x_running_indoor.fit";

        // Act
        var activity = ReadUtils.ReadFile(filePath);

        // Assert
        activity.CourseCharacteristics.TotalDistance.Meters.Should().BeApproximately(10540, 1);
    }
}
