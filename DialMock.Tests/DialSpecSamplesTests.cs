using DialMock.Core.Samples;

namespace DialMock.Tests;

public class DialSpecSamplesTests
{
    [Fact]
    public void CreateDefault_Returns_Expected_Default_Spec()
    {
        var spec = DialSpecSamples.CreateDefault();

        Assert.Equal("Pressure", spec.Title);
        Assert.Equal("bar", spec.Unit);
        Assert.Equal(0, spec.MinValue);
        Assert.Equal(10, spec.MaxValue);
        Assert.Equal(6, spec.PreviewValue);
        Assert.Equal(10, spec.MajorTickCount);
    }

    [Fact]
    public void CreatePressure100Bar_Returns_Expected_Spec()
    {
        var spec = DialSpecSamples.CreatePressure100Bar();

        Assert.Equal("Pressure", spec.Title);
        Assert.Equal("bar", spec.Unit);
        Assert.Equal(0, spec.MinValue);
        Assert.Equal(100, spec.MaxValue);
        Assert.Equal(50, spec.PreviewValue);
        Assert.Equal(10, spec.MajorTickCount);
    }
}