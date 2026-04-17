using DialMock.CadModel.Geometry;

namespace DialMock.CadModel.Model;

public sealed record CadCircle : CadEntity
{
    public required CadPoint2 Center { get; init; }
    public required double Radius { get; init; }
}