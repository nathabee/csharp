using DialAutoCADPlugin.Models;
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
    private const string DialCenterLayer = "DIAL_CENTER";
    private const string DialMetaLayer = "DIAL_META";

    public CadDrawing Map(DialDrawing drawing, DialCadRequest request)
    {
        ArgumentNullException.ThrowIfNull(drawing);
        ArgumentNullException.ThrowIfNull(request);

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
                RotationDeg = 0.0,
                Height = 8.0,
                LayerName = DialLabelsLayer
            });
        }

        entities.Add(new CadCircle
        {
            Center = new CadPoint2(0, 0),
            Radius = 6.0,
            LayerName = DialCenterLayer
        });

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            entities.Add(new CadText
            {
                Position = new CadPoint2(0, -28),
                Content = request.Title.Trim(),
                Height = 10.0,
                RotationDeg = 0.0,
                LayerName = DialMetaLayer
            });
        }

        if (!string.IsNullOrWhiteSpace(request.Unit))
        {
            entities.Add(new CadText
            {
                Position = new CadPoint2(0, -42),
                Content = request.Unit.Trim(),
                Height = 8.0,
                RotationDeg = 0.0,
                LayerName = DialMetaLayer
            });
        }

        return new CadDrawing
        {
            Layers =
            [
                new CadLayer { Name = DialArcLayer },
                new CadLayer { Name = DialTicksLayer },
                new CadLayer { Name = DialLabelsLayer },
                new CadLayer { Name = DialNeedleLayer },
                new CadLayer { Name = DialCenterLayer },
                new CadLayer { Name = DialMetaLayer }
            ],
            Entities = entities
        };
    }

    private static CadPoint2 ToCadPoint(Point2 point)
    {
        return new CadPoint2(point.X, point.Y);
    }
}