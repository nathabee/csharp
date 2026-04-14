using DialMock.Models;

namespace DialMock.Services;

public class DialRuleEngine
{
    public ValidationResult Validate(DialSpec spec)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(spec.Title))
        {
            errors.Add("Title is required.");
        }

        if (string.IsNullOrWhiteSpace(spec.Unit))
        {
            errors.Add("Unit is required.");
        }

        if (spec.MaxValue <= spec.MinValue)
        {
            errors.Add("MaxValue must be greater than MinValue.");
        }

        if (spec.PreviewValue < spec.MinValue || spec.PreviewValue > spec.MaxValue)
        {
            errors.Add("PreviewValue must stay inside the dial range.");
        }

        if (spec.MajorTickCount < 2 || spec.MajorTickCount > 20)
        {
            errors.Add("MajorTickCount must be between 2 and 20.");
        }

        return new ValidationResult(errors);
    }

    public DialRenderData BuildRenderData(DialSpec spec)
    {
        return new DialRenderData
        {
            Title = spec.Title.Trim(),
            Unit = spec.Unit.Trim(),
            MinValue = spec.MinValue,
            MaxValue = spec.MaxValue,
            PreviewValue = spec.PreviewValue,
            MajorTickCount = spec.MajorTickCount
        };
    }
}