namespace DialMock.CadModel.Model;

public abstract record CadEntity
{
    public string LayerName { get; init; } = "0";
}