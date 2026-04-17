using AutoCadMock.Diagnostics;
using DialAutoCADPlugin.Abstractions;
using DialAutoCADPlugin.Services;
using DialMock.Core.Samples;

var outputDirectory = Path.Combine(AppContext.BaseDirectory, "output");
Directory.CreateDirectory(outputDirectory);

var summaryPath = Path.Combine(outputDirectory, "cad-summary.txt");

IDialCadBuilder builder = new DialCadBuilder();

var spec = DialSpecSamples.CreateDefault();
var drawing = builder.Build(spec);

var summary = CadDrawingSummaryWriter.BuildSummary(drawing);

Console.WriteLine(summary);
File.WriteAllText(summaryPath, summary);

Console.WriteLine($"Summary written to: {summaryPath}");