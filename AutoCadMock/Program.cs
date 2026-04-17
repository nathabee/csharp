using AutoCadMock.Diagnostics;
using DialAutoCADPlugin.Abstractions;
using DialAutoCADPlugin.Export;
using DialAutoCADPlugin.Models;
using DialAutoCADPlugin.Services;

var outputDirectory = Path.Combine(AppContext.BaseDirectory, "output");
Directory.CreateDirectory(outputDirectory);

var summaryPath = Path.Combine(outputDirectory, "cad-summary.txt");
var dxfPath = Path.Combine(outputDirectory, "dial-output.dxf");

IDialCadBuilder builder = new DialCadBuilder();
ICadDrawingExporter exporter = new DxfCadDrawingExporter();

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

var summary = CadDrawingSummaryWriter.BuildSummary(drawing);

Console.WriteLine(summary);
File.WriteAllText(summaryPath, summary);

exporter.ExportToFile(drawing, dxfPath);

Console.WriteLine($"Summary written to: {summaryPath}");
Console.WriteLine($"DXF written to: {dxfPath}");