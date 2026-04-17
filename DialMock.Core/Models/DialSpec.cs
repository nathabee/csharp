using System.ComponentModel.DataAnnotations;

namespace DialMock.Core.Models;

public class DialSpec
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Unit { get; set; } = string.Empty;

    public double MinValue { get; set; }

    public double MaxValue { get; set; }

    public double PreviewValue { get; set; }

    [Range(2, 20)]
    public int MajorTickCount { get; set; }
}