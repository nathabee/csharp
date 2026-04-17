using DialMock.CadModel.Geometry;

namespace DialMock.CadModel.Model;

public sealed record CadText : CadEntity
{
    public required CadPoint2 Position { get; init; }
    public required string Content { get; init; }
    public double Height { get; init; } = 1.0;
    public double RotationDeg { get; init; } = 0.0;
}