using DialMock.CadModel.Geometry;

namespace DialMock.CadModel.Model;

public sealed record CadArc : CadEntity
{
    public required CadPoint2 Center { get; init; }
    public required double Radius { get; init; }
    public required double StartAngleDeg { get; init; }
    public required double EndAngleDeg { get; init; }
}