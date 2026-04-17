using AutoCadMock.Diagnostics;
using DialAutoCADPlugin.Abstractions;
using DialAutoCADPlugin.Models;
using DialAutoCADPlugin.Services;

var outputDirectory = Path.Combine(AppContext.BaseDirectory, "output");
Directory.CreateDirectory(outputDirectory);

var summaryPath = Path.Combine(outputDirectory, "cad-summary.txt");

IDialCadBuilder builder = new DialCadBuilder();

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

Console.WriteLine($"Summary written to: {summaryPath}");