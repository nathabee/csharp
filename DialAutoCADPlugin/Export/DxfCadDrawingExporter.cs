using System.Globalization;
using System.Text;
using DialAutoCADPlugin.Abstractions;
using DialMock.CadModel.Model;

namespace DialAutoCADPlugin.Export;

public sealed class DxfCadDrawingExporter : ICadDrawingExporter
{
    public string ExportToString(CadDrawing drawing)
    {
        ArgumentNullException.ThrowIfNull(drawing);

        var sb = new StringBuilder();

        WriteHeader(sb);
        WriteTables(sb, drawing);
        WriteEntities(sb, drawing);
        WriteEndOfFile(sb);

        return sb.ToString();
    }

    public void ExportToFile(CadDrawing drawing, string filePath)
    {
        ArgumentNullException.ThrowIfNull(drawing);

        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("DXF output path is required.", nameof(filePath));
        }

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(filePath, ExportToString(drawing), Encoding.ASCII);
    }

    private static void WriteHeader(StringBuilder sb)
    {
        WritePair(sb, 0, "SECTION");
        WritePair(sb, 2, "HEADER");
        WritePair(sb, 9, "$ACADVER");
        WritePair(sb, 1, "AC1009");
        WritePair(sb, 0, "ENDSEC");
    }

    private static void WriteTables(StringBuilder sb, CadDrawing drawing)
    {
        WritePair(sb, 0, "SECTION");
        WritePair(sb, 2, "TABLES");

        WritePair(sb, 0, "TABLE");
        WritePair(sb, 2, "LAYER");
        WritePair(sb, 70, drawing.Layers.Count);

        foreach (var layer in drawing.Layers)
        {
            WritePair(sb, 0, "LAYER");
            WritePair(sb, 2, layer.Name);
            WritePair(sb, 70, 0);
            WritePair(sb, 62, 7);
            WritePair(sb, 6, "CONTINUOUS");
        }

        WritePair(sb, 0, "ENDTAB");
        WritePair(sb, 0, "ENDSEC");
    }

    private static void WriteEntities(StringBuilder sb, CadDrawing drawing)
    {
        WritePair(sb, 0, "SECTION");
        WritePair(sb, 2, "ENTITIES");

        foreach (var entity in drawing.Entities)
        {
            switch (entity)
            {
                case CadLine line:
                    WriteLine(sb, line);
                    break;

                case CadArc arc:
                    WriteArc(sb, arc);
                    break;

                case CadCircle circle:
                    WriteCircle(sb, circle);
                    break;

                case CadText text:
                    WriteText(sb, text);
                    break;
            }
        }

        WritePair(sb, 0, "ENDSEC");
    }

    private static void WriteLine(StringBuilder sb, CadLine line)
    {
        WritePair(sb, 0, "LINE");
        WritePair(sb, 8, line.LayerName);
        WritePair(sb, 10, F(line.Start.X));
        WritePair(sb, 20, F(line.Start.Y));
        WritePair(sb, 30, F(0));
        WritePair(sb, 11, F(line.End.X));
        WritePair(sb, 21, F(line.End.Y));
        WritePair(sb, 31, F(0));
    }

    private static void WriteArc(StringBuilder sb, CadArc arc)
    {
        WritePair(sb, 0, "ARC");
        WritePair(sb, 8, arc.LayerName);
        WritePair(sb, 10, F(arc.Center.X));
        WritePair(sb, 20, F(arc.Center.Y));
        WritePair(sb, 30, F(0));
        WritePair(sb, 40, F(arc.Radius));
        WritePair(sb, 50, F(NormalizeAngle(arc.StartAngleDeg)));
        WritePair(sb, 51, F(NormalizeAngle(arc.EndAngleDeg)));
    }

    private static void WriteCircle(StringBuilder sb, CadCircle circle)
    {
        WritePair(sb, 0, "CIRCLE");
        WritePair(sb, 8, circle.LayerName);
        WritePair(sb, 10, F(circle.Center.X));
        WritePair(sb, 20, F(circle.Center.Y));
        WritePair(sb, 30, F(0));
        WritePair(sb, 40, F(circle.Radius));
    }

    private static void WriteText(StringBuilder sb, CadText text)
    {
        WritePair(sb, 0, "TEXT");
        WritePair(sb, 8, text.LayerName);
        WritePair(sb, 10, F(text.Position.X));
        WritePair(sb, 20, F(text.Position.Y));
        WritePair(sb, 30, F(0));
        WritePair(sb, 40, F(text.Height));
        WritePair(sb, 1, text.Content);
        WritePair(sb, 50, F(text.RotationDeg));
        WritePair(sb, 72, 1);
        WritePair(sb, 73, 2);
        WritePair(sb, 11, F(text.Position.X));
        WritePair(sb, 21, F(text.Position.Y));
        WritePair(sb, 31, F(0));
    }

    private static void WriteEndOfFile(StringBuilder sb)
    {
        WritePair(sb, 0, "EOF");
    }

    private static void WritePair(StringBuilder sb, int code, string value)
    {
        sb.AppendLine(code.ToString(CultureInfo.InvariantCulture));
        sb.AppendLine(value);
    }

    private static void WritePair(StringBuilder sb, int code, int value)
    {
        sb.AppendLine(code.ToString(CultureInfo.InvariantCulture));
        sb.AppendLine(value.ToString(CultureInfo.InvariantCulture));
    }

    private static string F(double value)
    {
        return value.ToString("0.###", CultureInfo.InvariantCulture);
    }

    private static double NormalizeAngle(double angle)
    {
        var normalized = angle % 360.0;
        return normalized < 0 ? normalized + 360.0 : normalized;
    }
}