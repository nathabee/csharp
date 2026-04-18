using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using AutoCadMock.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DialAutoCADPlugin.Abstractions;
using DialAutoCADPlugin.Export;
using DialAutoCADPlugin.Models;
using DialAutoCADPlugin.Services;

namespace AutoCadMock.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IDialCadBuilder _builder = new DialCadBuilder();
    private readonly ICadDrawingExporter _exporter = new DxfCadDrawingExporter();

    [ObservableProperty]
    private string title = "Pressure";

    [ObservableProperty]
    private string unit = "bar";

    [ObservableProperty]
    private string minValue = "0";

    [ObservableProperty]
    private string maxValue = "10";

    [ObservableProperty]
    private string previewValue = "6";

    [ObservableProperty]
    private string majorTickCount = "10";

    [ObservableProperty]
    private string outputPath = BuildDefaultOutputPath();

    [ObservableProperty]
    private string statusMessage = "Ready.";

    [ObservableProperty]
    private string summaryText = string.Empty;

    [RelayCommand]
    private void LoadDefaultSample()
    {
        Title = "Pressure";
        Unit = "bar";
        MinValue = "0";
        MaxValue = "10";
        PreviewValue = "6";
        MajorTickCount = "10";
        StatusMessage = "Loaded default sample.";
    }

    [RelayCommand]
    private void LoadPressure100Sample()
    {
        Title = "Pressure";
        Unit = "bar";
        MinValue = "0";
        MaxValue = "100";
        PreviewValue = "50";
        MajorTickCount = "10";
        StatusMessage = "Loaded pressure100 sample.";
    }

    [RelayCommand]
    private void GenerateDxf()
    {
        try
        {
            var request = BuildRequest();
            var drawing = _builder.Build(request);

            SummaryText = CadDrawingSummaryWriter.BuildSummary(drawing);

            var directory = Path.GetDirectoryName(OutputPath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            _exporter.ExportToFile(drawing, OutputPath);

            StatusMessage = $"DXF generated: {OutputPath}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
    }

    [RelayCommand]
    private void OpenOutputFolder()
    {
        try
        {
            var directory = Path.GetDirectoryName(OutputPath);

            if (string.IsNullOrWhiteSpace(directory))
            {
                StatusMessage = "No output folder is defined.";
                return;
            }

            Directory.CreateDirectory(directory);

            var psi = new ProcessStartInfo
            {
                FileName = directory,
                UseShellExecute = true
            };

            Process.Start(psi);
            StatusMessage = $"Opened folder: {directory}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error opening folder: {ex.Message}";
        }
    }

    private DialCadRequest BuildRequest()
    {
        return new DialCadRequest
        {
            Title = Title.Trim(),
            Unit = Unit.Trim(),
            MinValue = ParseDouble(MinValue, nameof(MinValue)),
            MaxValue = ParseDouble(MaxValue, nameof(MaxValue)),
            PreviewValue = ParseDouble(PreviewValue, nameof(PreviewValue)),
            MajorTickCount = ParseInt(MajorTickCount, nameof(MajorTickCount))
        };
    }

    private static string BuildDefaultOutputPath()
    {
        var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        if (string.IsNullOrWhiteSpace(documents))
        {
            documents = AppContext.BaseDirectory;
        }

        return Path.Combine(documents, "AutoCadMock", "dial-output.dxf");
    }

    private static double ParseDouble(string value, string fieldName)
    {
        if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
        {
            throw new InvalidOperationException($"{fieldName} must be a valid number.");
        }

        return result;
    }

    private static int ParseInt(string value, string fieldName)
    {
        if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
        {
            throw new InvalidOperationException($"{fieldName} must be a valid integer.");
        }

        return result;
    }
}