using DialMock.CadModel.Model;
using DialMock.Core.Models;

namespace DialAutoCADPlugin.Abstractions;

public interface IDialCadBuilder
{
    CadDrawing Build(DialSpec spec);
}