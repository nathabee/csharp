namespace DialAutoCADPlugin.Models;

public class DialCadRequest
{
    public string Title { get; set; } = string.Empty;

    public string Unit { get; set; } = string.Empty;

    public double MinValue { get; set; }

    public double MaxValue { get; set; }

    public double PreviewValue { get; set; }

    public int MajorTickCount { get; set; }
}