namespace DialMock.CadModel.Model;

public sealed record CadDrawing
{
    public IReadOnlyList<CadLayer> Layers { get; init; } = Array.Empty<CadLayer>();
    public IReadOnlyList<CadEntity> Entities { get; init; } = Array.Empty<CadEntity>();
}