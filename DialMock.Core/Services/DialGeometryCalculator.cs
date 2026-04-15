using DialMock.Core.Models;

namespace DialMock.Core.Services;

public class DialGeometryCalculator
{
    private const double StartAngle = -160;
    private const double SweepAngle = 140;

    public DialLayoutData Calculate(DialSpec spec)
    {
        var layout = new DialLayoutData();

        double range = spec.MaxValue - spec.MinValue;
        double percent = (spec.PreviewValue - spec.MinValue) / range;

        layout.NeedleAngleDeg = StartAngle + percent * SweepAngle;

        for (int i = 0; i <= spec.MajorTickCount; i++)
        {
            double tickPercent = (double)i / spec.MajorTickCount;
            double value = spec.MinValue + tickPercent * range;
            double angle = StartAngle + tickPercent * SweepAngle;

            layout.MajorTicks.Add(new DialLayoutData.TickMark
            {
                Value = value,
                AngleDeg = angle
            });
        }

        return layout;
    }
}