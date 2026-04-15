namespace DialMock.Core.Geometry;

public class DialDrawing
{
    public List<Line2> Lines { get; } = new();
    public List<Arc2> Arcs { get; } = new();
    public List<Text2> Texts { get; } = new();
}