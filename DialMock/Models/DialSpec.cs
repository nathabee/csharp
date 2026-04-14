using System.ComponentModel.DataAnnotations;

namespace DialMock.Models;

public class DialSpec
{
    [Required]
    public string Title { get; set; } = "Pressure";

    [Required]
    public string Unit { get; set; } = "bar";

    public double MinValue { get; set; } = 0;

    public double MaxValue { get; set; } = 10;

    public double PreviewValue { get; set; } = 6;

    [Range(2, 20)]
    public int MajorTickCount { get; set; } = 10;
}