using DialMock.CadModel.Model;

namespace DialAutoCADPlugin.Abstractions;

public interface ICadDrawingExporter
{
    string ExportToString(CadDrawing drawing);
    void ExportToFile(CadDrawing drawing, string filePath);
}