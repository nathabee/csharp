using DialMock.CadModel.Geometry;
using DialMock.CadModel.Model;
using DialMock.Core.Geometry;

namespace DialAutoCADPlugin.Mapping;

internal sealed class DialDrawingToCadMapper
{
    private const string DialArcLayer = "DIAL_ARC";
    private const string DialTicksLayer = "DIAL_TICKS";
    private const string DialLabelsLayer = "DIAL_LABELS";
    private const string DialNeedleLayer = "DIAL_NEEDLE";

    public CadDrawing Map(DialDrawing drawing)
    {
        ArgumentNullException.ThrowIfNull(drawing);

        var entities = new List<CadEntity>();

        foreach (var arc in drawing.Arcs)
        {
            entities.Add(new CadArc
            {
                Center = ToCadPoint(arc.Center),
                Radius = arc.Radius,
                StartAngleDeg = arc.StartAngleDeg,
                SweepAngleDeg = arc.SweepAngleDeg,
                LayerName = DialArcLayer
            });
        }

        for (int i = 0; i < drawing.Lines.Count; i++)
        {
            var line = drawing.Lines[i];
            var layerName = i == drawing.Lines.Count - 1
                ? DialNeedleLayer
                : DialTicksLayer;

            entities.Add(new CadLine
            {
                Start = ToCadPoint(line.Start),
                End = ToCadPoint(line.End),
                LayerName = layerName
            });
        }

        foreach (var text in drawing.Texts)
        {
            entities.Add(new CadText
            {
                Position = ToCadPoint(text.Position),
                Content = text.Content,
                RotationDeg = text.RotationDeg,
                Height = 8.0,
                LayerName = DialLabelsLayer
            });
        }

        return new CadDrawing
        {
            Layers =
            [
                new CadLayer { Name = DialArcLayer },
                new CadLayer { Name = DialTicksLayer },
                new CadLayer { Name = DialLabelsLayer },
                new CadLayer { Name = DialNeedleLayer }
            ],
            Entities = entities
        };
    }

    private static CadPoint2 ToCadPoint(Point2 point)
    {
        return new CadPoint2(point.X, point.Y);
    }
}