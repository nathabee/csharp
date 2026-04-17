using DialAutoCADPlugin.Models;
using DialMock.CadModel.Model;

namespace DialAutoCADPlugin.Abstractions;

public interface IDialCadBuilder
{
    CadDrawing Build(DialCadRequest request);
}