using DialMock.Core.Engine;
using DialMock.Core.Models;
using Xunit;

namespace DialMock.Tests;

public class DialEngineTests
{
    [Fact]
    public void BuildDrawing_Creates_One_Arc()
    {
        var engine = new DialEngine();
        var spec = new DialSpec
        {
            Title = "Pressure",
            Unit = "bar",
            MinValue = 0,
            MaxValue = 100,
            PreviewValue = 50,
            MajorTickCount = 10
        };

        var drawing = engine.BuildDrawing(spec);

        Assert.Single(drawing.Arcs);
        Assert.Equal(160, drawing.Arcs[0].Radius);
        Assert.Equal(160, drawing.Arcs[0].StartAngleDeg);
        Assert.Equal(-140, drawing.Arcs[0].SweepAngleDeg);
    }

    [Fact]
    public void BuildDrawing_Creates_Expected_Number_Of_Tick_Lines_And_One_Needle()
    {
        var engine = new DialEngine();
        var spec = new DialSpec
        {
            Title = "Pressure",
            Unit = "bar",
            MinValue = 0,
            MaxValue = 100,
            PreviewValue = 50,
            MajorTickCount = 10
        };

        var drawing = engine.BuildDrawing(spec);

        // 11 tick lines (0..10) + 1 needle
        Assert.Equal(12, drawing.Lines.Count);
    }

    [Fact]
    public void BuildDrawing_Creates_Expected_Number_Of_Labels()
    {
        var engine = new DialEngine();
        var spec = new DialSpec
        {
            Title = "Pressure",
            Unit = "bar",
            MinValue = 0,
            MaxValue = 100,
            PreviewValue = 50,
            MajorTickCount = 10
        };

        var drawing = engine.BuildDrawing(spec);

        Assert.Equal(11, drawing.Texts.Count);
    }

    [Fact]
    public void BuildDrawing_Middle_Value_Places_Needle_At_Top()
    {
        var engine = new DialEngine();
        var spec = new DialSpec
        {
            Title = "Pressure",
            Unit = "bar",
            MinValue = 0,
            MaxValue = 100,
            PreviewValue = 50,
            MajorTickCount = 10
        };

        var drawing = engine.BuildDrawing(spec);

        var needle = drawing.Lines[^1];

        Assert.Equal(0, needle.Start.X, 6);
        Assert.Equal(0, needle.Start.Y, 6);

        Assert.Equal(0, needle.End.X, 6);
        Assert.Equal(114, needle.End.Y, 6);
    }

    [Fact]
    public void BuildDrawing_Min_Value_Places_Needle_On_Left_Upper_Side()
    {
        var engine = new DialEngine();
        var spec = new DialSpec
        {
            Title = "Pressure",
            Unit = "bar",
            MinValue = 0,
            MaxValue = 100,
            PreviewValue = 0,
            MajorTickCount = 10
        };

        var drawing = engine.BuildDrawing(spec);

        var needle = drawing.Lines[^1];

        Assert.True(needle.End.X < 0);
        Assert.True(needle.End.Y > 0);
    }

    [Fact]
    public void BuildDrawing_Max_Value_Places_Needle_On_Right_Upper_Side()
    {
        var engine = new DialEngine();
        var spec = new DialSpec
        {
            Title = "Pressure",
            Unit = "bar",
            MinValue = 0,
            MaxValue = 100,
            PreviewValue = 100,
            MajorTickCount = 10
        };

        var drawing = engine.BuildDrawing(spec);

        var needle = drawing.Lines[^1];

        Assert.True(needle.End.X > 0);
        Assert.True(needle.End.Y > 0);
    }
}