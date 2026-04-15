using DialMock.Core.Models;
using DialMock.Core.Services;
using Xunit;

namespace DialMock.Tests;

public class DialRuleEngineTests
{
    [Fact]
    public void Validate_Returns_Valid_For_Correct_Spec()
    {
        var engine = new DialRuleEngine();
        var spec = new DialSpec
        {
            Title = "Pressure",
            Unit = "bar",
            MinValue = 0,
            MaxValue = 100,
            PreviewValue = 50,
            MajorTickCount = 10
        };

        var result = engine.Validate(spec);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_Returns_Invalid_When_Max_Is_Not_Greater_Than_Min()
    {
        var engine = new DialRuleEngine();
        var spec = new DialSpec
        {
            Title = "Pressure",
            Unit = "bar",
            MinValue = 100,
            MaxValue = 100,
            PreviewValue = 100,
            MajorTickCount = 10
        };

        var result = engine.Validate(spec);

        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Validate_Returns_Invalid_When_Preview_Is_Out_Of_Range()
    {
        var engine = new DialRuleEngine();
        var spec = new DialSpec
        {
            Title = "Pressure",
            Unit = "bar",
            MinValue = 0,
            MaxValue = 100,
            PreviewValue = 200,
            MajorTickCount = 10
        };

        var result = engine.Validate(spec);

        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Validate_Returns_Invalid_When_MajorTickCount_Is_Too_Low()
    {
        var engine = new DialRuleEngine();
        var spec = new DialSpec
        {
            Title = "Pressure",
            Unit = "bar",
            MinValue = 0,
            MaxValue = 100,
            PreviewValue = 50,
            MajorTickCount = 1
        };

        var result = engine.Validate(spec);

        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }
}