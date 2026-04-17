using DialMock.CadModel.Geometry;

namespace DialMock.CadModel.Model;

public sealed record CadLine : CadEntity
{
    public required CadPoint2 Start { get; init; }
    public required CadPoint2 End { get; init; }
}