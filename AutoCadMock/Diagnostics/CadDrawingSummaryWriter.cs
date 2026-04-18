using System;
using System.Linq;
using System.Text;
using DialMock.CadModel.Model;

namespace AutoCadMock.Diagnostics;

internal static class CadDrawingSummaryWriter
{
    public static string BuildSummary(CadDrawing drawing)
    {
        ArgumentNullException.ThrowIfNull(drawing);

        var sb = new StringBuilder();

        sb.AppendLine("CAD DRAWING SUMMARY");
        sb.AppendLine("===================");
        sb.AppendLine($"Layers   : {drawing.Layers.Count}");
        sb.AppendLine($"Entities : {drawing.Entities.Count}");
        sb.AppendLine();

        sb.AppendLine("Layers:");
        foreach (var layer in drawing.Layers)
        {
            sb.AppendLine($"- {layer.Name}");
        }

        sb.AppendLine();
        sb.AppendLine("Entities by type:");

        var groupedByType = drawing.Entities
            .GroupBy(e => e.GetType().Name)
            .OrderBy(g => g.Key);

        foreach (var group in groupedByType)
        {
            sb.AppendLine($"- {group.Key}: {group.Count()}");
        }

        sb.AppendLine();
        sb.AppendLine("Entities by layer:");

        var groupedByLayer = drawing.Entities
            .GroupBy(e => e.LayerName)
            .OrderBy(g => g.Key);

        foreach (var group in groupedByLayer)
        {
            sb.AppendLine($"- {group.Key}: {group.Count()}");
        }

        return sb.ToString();
    }
}