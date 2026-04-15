using DialMock.Core.Geometry;
using DialMock.Core.Models;

namespace DialMock.Core.Engine;

public class DialEngine
{
    private const double StartAngle = 160;
    private const double SweepAngle = -140;

    public DialDrawing BuildDrawing(DialSpec spec)
    {
        var drawing = new DialDrawing();

        double range = spec.MaxValue - spec.MinValue;
        if (range <= 0 || spec.MajorTickCount <= 0)
        {
            return drawing;
        }

        drawing.Arcs.Add(new Arc2(
            new Point2(0, 0),
            160,
            StartAngle,
            SweepAngle));

        for (int i = 0; i <= spec.MajorTickCount; i++)
        {
            double tickPercent = (double)i / spec.MajorTickCount;
            double value = spec.MinValue + tickPercent * range;
            double angle = StartAngle + tickPercent * SweepAngle;

            var outer = Polar(new Point2(0, 0), 142, angle);
            var inner = Polar(new Point2(0, 0), 126, angle);
            var labelPos = Polar(new Point2(0, 0), 108, angle);

            drawing.Lines.Add(new Line2(inner, outer));
            drawing.Texts.Add(new Text2(labelPos, value.ToString("0")));
        }

        double needlePercent = (spec.PreviewValue - spec.MinValue) / range;
        double needleAngle = StartAngle + needlePercent * SweepAngle;

        drawing.Lines.Add(new Line2(
            new Point2(0, 0),
            Polar(new Point2(0, 0), 114, needleAngle)));

        return drawing;
    }

    private static Point2 Polar(Point2 center, double radius, double angleDegrees)
    {
        double radians = Math.PI * angleDegrees / 180.0;

        return new Point2(
            center.X + radius * Math.Cos(radians),
            center.Y + radius * Math.Sin(radians));
    }
}