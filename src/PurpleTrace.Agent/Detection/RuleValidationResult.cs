namespace PurpleTrace.Agent.Detection;

public sealed class RuleValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<string> Errors { get; } = new();

    public void AddError(string error)
    {
        Errors.Add(error);
    }
}
