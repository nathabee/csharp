using DialAutoCADPlugin.Export;
using DialAutoCADPlugin.Models;
using DialAutoCADPlugin.Services;

namespace DialMock.Tests;

public class DxfCadDrawingExporterTests
{
    [Fact]
    public void ExportToString_Contains_Expected_Dxf_Sections()
    {
        var builder = new DialCadBuilder();
        var exporter = new DxfCadDrawingExporter();

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
        var dxf = exporter.ExportToString(drawing);

        Assert.Contains("SECTION", dxf);
        Assert.Contains("HEADER", dxf);
        Assert.Contains("TABLES", dxf);
        Assert.Contains("ENTITIES", dxf);
        Assert.Contains("EOF", dxf);
    }

    [Fact]
    public void ExportToString_Contains_Expected_Layers()
    {
        var builder = new DialCadBuilder();
        var exporter = new DxfCadDrawingExporter();

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
        var dxf = exporter.ExportToString(drawing);

        Assert.Contains("DIAL_ARC", dxf);
        Assert.Contains("DIAL_TICKS", dxf);
        Assert.Contains("DIAL_LABELS", dxf);
        Assert.Contains("DIAL_NEEDLE", dxf);
        Assert.Contains("DIAL_CENTER", dxf);
        Assert.Contains("DIAL_META", dxf);
    }

    [Fact]
    public void ExportToString_Contains_Expected_Entity_Types()
    {
        var builder = new DialCadBuilder();
        var exporter = new DxfCadDrawingExporter();

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
        var dxf = exporter.ExportToString(drawing);

        Assert.Contains("ARC", dxf);
        Assert.Contains("LINE", dxf);
        Assert.Contains("CIRCLE", dxf);
        Assert.Contains("TEXT", dxf);
    }

    [Fact]
    public void ExportToString_Writes_Normalized_Arc_Angles()
    {
        var builder = new DialCadBuilder();
        var exporter = new DxfCadDrawingExporter();

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
        var dxf = exporter.ExportToString(drawing);

        Assert.Contains("50\n20", dxf.Replace("\r\n", "\n"));
        Assert.Contains("51\n160", dxf.Replace("\r\n", "\n"));
    }

    [Fact]
    public void ExportToFile_Creates_Dxf_File()
    {
        var builder = new DialCadBuilder();
        var exporter = new DxfCadDrawingExporter();

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

        var tempFile = Path.Combine(Path.GetTempPath(), $"dialmock-{Guid.NewGuid():N}.dxf");

        try
        {
            exporter.ExportToFile(drawing, tempFile);

            Assert.True(File.Exists(tempFile));

            var content = File.ReadAllText(tempFile);
            Assert.Contains("EOF", content);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }
}