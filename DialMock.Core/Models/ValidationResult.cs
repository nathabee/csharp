namespace DialMock.Core.Models;

public class ValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public IReadOnlyList<string> Errors { get; }

    public ValidationResult(IEnumerable<string> errors)
    {
        Errors = errors.ToList();
    }
}