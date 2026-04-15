using System.Net;
using System.Text;
using DialMock.Core.Geometry;
using DialMock.Core.Models;

namespace DialMock.Rendering;

public class SvgDialRenderer
{
    private const double SvgCenterX = 200;
    private const double SvgCenterY = 200;

    public string Render(DialDrawing drawing, DialRenderData renderData)
    {
        var sb = new StringBuilder();

        foreach (var arc in drawing.Arcs)
        {
            sb.AppendLine($"""<path d="{SvgArcPath(arc)}" class="dial-arc" fill="none"/>""");
        }

        for (int i = 0; i < drawing.Lines.Count; i++)
        {
            var line = drawing.Lines[i];
            var cssClass = i == drawing.Lines.Count - 1 ? "dial-needle" : "dial-tick";

            sb.AppendLine(
                $"""<line x1="{SvgX(line.Start.X):0.##}" y1="{SvgY(line.Start.Y):0.##}" x2="{SvgX(line.End.X):0.##}" y2="{SvgY(line.End.Y):0.##}" class="{cssClass}" />"""
            );
        }

        sb.AppendLine("""<circle cx="200" cy="200" r="8" class="dial-center" />""");

        foreach (var text in drawing.Texts)
        {
            sb.AppendLine(SvgText(
                SvgX(text.Position.X),
                SvgY(text.Position.Y),
                text.Content,
                "dial-label"));
        }

        sb.AppendLine(SvgText(200, 30, renderData.Title, "dial-title"));
        sb.AppendLine(SvgText(200, 224, renderData.Unit, "dial-unit"));

        return sb.ToString();
    }

    private static double SvgX(double x) => SvgCenterX + x;

    private static double SvgY(double y) => SvgCenterY - y;

    private static string SvgArcPath(Arc2 arc)
    {
        var start = Polar(arc.Center, arc.Radius, arc.StartAngleDeg);
        var end = Polar(arc.Center, arc.Radius, arc.StartAngleDeg + arc.SweepAngleDeg);

        var largeArcFlag = Math.Abs(arc.SweepAngleDeg) > 180 ? 1 : 0;
        var sweepFlag = arc.SweepAngleDeg >= 0 ? 0 : 1;

        return $"M {SvgX(start.X):0.##} {SvgY(start.Y):0.##} " +
               $"A {arc.Radius:0.##} {arc.Radius:0.##} 0 {largeArcFlag} {sweepFlag} {SvgX(end.X):0.##} {SvgY(end.Y):0.##}";
    }

    private static Point2 Polar(Point2 center, double radius, double angleDegrees)
    {
        var radians = Math.PI * angleDegrees / 180.0;

        return new Point2(
            center.X + radius * Math.Cos(radians),
            center.Y + radius * Math.Sin(radians)
        );
    }

    private static string SvgText(double x, double y, string content, string cssClass)
    {
        var encoded = WebUtility.HtmlEncode(content);

        return $"""<text x="{x:0.##}" y="{y:0.##}" class="{cssClass}" text-anchor="middle">{encoded}</text>""";
    }
}