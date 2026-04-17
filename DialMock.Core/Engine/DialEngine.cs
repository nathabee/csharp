using DialMock.Core.Geometry;
using DialMock.Core.Models;

namespace DialMock.Core.Engine;

public class DialEngine
{
    // CAD-normal arc definition for the upper half.
    // Positive angles are measured counter-clockwise from +X.
    private const double ArcStartAngle = 20;
    private const double ArcEndAngle = 160;

    // Value progression still runs from left-upper (min) to right-upper (max).
    private const double ValueMinAngle = ArcEndAngle;
    private const double ValueMaxAngle = ArcStartAngle;

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
            ArcStartAngle,
            ArcEndAngle));

        for (int i = 0; i <= spec.MajorTickCount; i++)
        {
            double tickPercent = (double)i / spec.MajorTickCount;
            double value = spec.MinValue + tickPercent * range;
            double angle = ValueMinAngle + tickPercent * (ValueMaxAngle - ValueMinAngle);

            var outer = Polar(new Point2(0, 0), 142, angle);
            var inner = Polar(new Point2(0, 0), 126, angle);
            var labelPos = Polar(new Point2(0, 0), 108, angle);

            drawing.Lines.Add(new Line2(inner, outer));
            drawing.Texts.Add(new Text2(labelPos, value.ToString("0")));
        }

        double needlePercent = (spec.PreviewValue - spec.MinValue) / range;
        double needleAngle = ValueMinAngle + needlePercent * (ValueMaxAngle - ValueMinAngle);

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