using DialAutoCADPlugin.Models;
using DialAutoCADPlugin.Services;
using DialMock.CadModel.Model;

namespace DialMock.Tests;

public class DialCadBuilderTests
{
    [Fact]
    public void Build_Returns_CadDrawing_With_Expected_Layers_And_Entities()
    {
        var builder = new DialCadBuilder();

        var request = new DialCadRequest
        {
            Title = "Pressure",
            Unit = "bar",
            MinValue = 0,
            MaxValue = 10,
            PreviewValue = 6,
            MajorTickCount = 10
        };

        var drawing = builder.Build(request);

        Assert.NotNull(drawing);
        Assert.Equal(6, drawing.Layers.Count);
        Assert.Equal(27, drawing.Entities.Count);

        Assert.Contains(drawing.Layers, l => l.Name == "DIAL_ARC");
        Assert.Contains(drawing.Layers, l => l.Name == "DIAL_TICKS");
        Assert.Contains(drawing.Layers, l => l.Name == "DIAL_LABELS");
        Assert.Contains(drawing.Layers, l => l.Name == "DIAL_NEEDLE");
        Assert.Contains(drawing.Layers, l => l.Name == "DIAL_CENTER");
        Assert.Contains(drawing.Layers, l => l.Name == "DIAL_META");

        Assert.Single(drawing.Entities.OfType<CadArc>());
        Assert.Equal(12, drawing.Entities.OfType<CadLine>().Count());
        Assert.Equal(13, drawing.Entities.OfType<CadText>().Count());
        Assert.Single(drawing.Entities.OfType<CadCircle>());
    }

    [Fact]
    public void Build_Produces_Normalized_Arc_Angles()
    {
        var builder = new DialCadBuilder();

        var request = new DialCadRequest
        {
            Title = "Pressure",
            Unit = "bar",
            MinValue = 0,
            MaxValue = 100,
            PreviewValue = 50,
            MajorTickCount = 10
        };

        var drawing = builder.Build(request);
        var arc = Assert.Single(drawing.Entities.OfType<CadArc>());

        Assert.Equal(160, arc.Radius);
        Assert.Equal(20, arc.StartAngleDeg);
        Assert.Equal(160, arc.EndAngleDeg);
        Assert.Equal("DIAL_ARC", arc.LayerName);
    }

    [Fact]
    public void Build_Adds_Title_And_Unit_As_Meta_Text()
    {
        var builder = new DialCadBuilder();

        var request = new DialCadRequest
        {
            Title = "Pressure",
            Unit = "bar",
            MinValue = 0,
            MaxValue = 10,
            PreviewValue = 6,
            MajorTickCount = 10
        };

        var drawing = builder.Build(request);

        var metaTexts = drawing.Entities
            .OfType<CadText>()
            .Where(t => t.LayerName == "DIAL_META")
            .ToList();

        Assert.Equal(2, metaTexts.Count);
        Assert.Contains(metaTexts, t => t.Content == "Pressure");
        Assert.Contains(metaTexts, t => t.Content == "bar");
    }

    [Fact]
    public void Build_Throws_When_Request_Is_Invalid()
    {
        var builder = new DialCadBuilder();

        var request = new DialCadRequest
        {
            Title = "Pressure",
            Unit = "bar",
            MinValue = 100,
            MaxValue = 100,
            PreviewValue = 100,
            MajorTickCount = 10
        };

        var ex = Assert.Throws<InvalidOperationException>(() => builder.Build(request));

        Assert.Contains("invalid", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Build_Creates_One_Needle_On_Needle_Layer()
    {
        var builder = new DialCadBuilder();

        var request = new DialCadRequest
        {
            Title = "Pressure",
            Unit = "bar",
            MinValue = 0,
            MaxValue = 100,
            PreviewValue = 50,
            MajorTickCount = 10
        };

        var drawing = builder.Build(request);

        var needleLines = drawing.Entities
            .OfType<CadLine>()
            .Where(l => l.LayerName == "DIAL_NEEDLE")
            .ToList();

        Assert.Single(needleLines);
    }
}