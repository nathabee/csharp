using DialAutoCADPlugin.Abstractions;
using DialAutoCADPlugin.Mapping;
using DialAutoCADPlugin.Models;
using DialMock.CadModel.Model;
using DialMock.Core.Engine;
using DialMock.Core.Models;
using DialMock.Core.Services;

namespace DialAutoCADPlugin.Services;

public sealed class DialCadBuilder : IDialCadBuilder
{
    private readonly DialRuleEngine _ruleEngine;
    private readonly DialEngine _dialEngine;
    private readonly DialDrawingToCadMapper _mapper;

    public DialCadBuilder()
        : this(new DialRuleEngine(), new DialEngine(), new DialDrawingToCadMapper())
    {
    }

    internal DialCadBuilder(
        DialRuleEngine ruleEngine,
        DialEngine dialEngine,
        DialDrawingToCadMapper mapper)
    {
        _ruleEngine = ruleEngine ?? throw new ArgumentNullException(nameof(ruleEngine));
        _dialEngine = dialEngine ?? throw new ArgumentNullException(nameof(dialEngine));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public CadDrawing Build(DialCadRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = ToDialSpec(request);

        var validation = _ruleEngine.Validate(spec);
        if (!validation.IsValid)
        {
            throw new InvalidOperationException(
                $"DialCadRequest is invalid: {string.Join(" | ", validation.Errors)}");
        }

        var drawing = _dialEngine.BuildDrawing(spec);
        return _mapper.Map(drawing);
    }

    private static DialSpec ToDialSpec(DialCadRequest request)
    {
        return new DialSpec
        {
            Title = request.Title,
            Unit = request.Unit,
            MinValue = request.MinValue,
            MaxValue = request.MaxValue,
            PreviewValue = request.PreviewValue,
            MajorTickCount = request.MajorTickCount
        };
    }
}