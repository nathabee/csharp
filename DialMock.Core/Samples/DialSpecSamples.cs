using DialMock.Core.Models;

namespace DialMock.Core.Samples;

public static class DialSpecSamples
{
    public static DialSpec CreateDefault()
    {
        return new DialSpec
        {
            Title = "Pressure",
            Unit = "bar",
            MinValue = 0,
            MaxValue = 10,
            PreviewValue = 6,
            MajorTickCount = 10
        };
    }

    public static DialSpec CreatePressure100Bar()
    {
        return new DialSpec
        {
            Title = "Pressure",
            Unit = "bar",
            MinValue = 0,
            MaxValue = 100,
            PreviewValue = 50,
            MajorTickCount = 10
        };
    }
}