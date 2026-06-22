namespace PurpleTrace.Agent.Detection;

public static class RuleValidator
{
    private static readonly HashSet<string> ValidSeverities = new(StringComparer.OrdinalIgnoreCase)
    {
        "Critical",
        "High",
        "Medium",
        "Low",
        "Informational"
    };

    public static RuleValidationResult Validate(DetectionRule rule)
    {
        var result = new RuleValidationResult();

        rule.Tags ??= new List<string>();
        rule.References ??= new List<string>();
        rule.ProcessNameContains ??= new List<string>();
        rule.CommandLineContains ??= new List<string>();
        rule.ParentProcessNameContains ??= new List<string>();

        if (string.IsNullOrWhiteSpace(rule.Id))
        {
            result.AddError("Rule Id is required.");
        }

        if (string.IsNullOrWhiteSpace(rule.Title))
        {
            result.AddError("Rule Title is required.");
        }

        if (string.IsNullOrWhiteSpace(rule.Description))
        {
            result.AddError("Rule Description is required.");
        }

        if (string.IsNullOrWhiteSpace(rule.Severity))
        {
            result.AddError("Rule Severity is required.");
        }
        else if (!ValidSeverities.Contains(rule.Severity))
        {
            result.AddError($"Invalid Severity value: {rule.Severity}. Valid values: Critical, High, Medium, Low, Informational.");
        }

        if (string.IsNullOrWhiteSpace(rule.MitreTactic))
        {
            result.AddError("MITRE tactic is required.");
        }

        if (string.IsNullOrWhiteSpace(rule.MitreTechniqueId))
        {
            result.AddError("MITRE technique ID is required.");
        }

        if (string.IsNullOrWhiteSpace(rule.MitreTechniqueName))
        {
            result.AddError("MITRE technique name is required.");
        }

        var hasAnyCondition =
            rule.ProcessNameContains.Count > 0 ||
            rule.CommandLineContains.Count > 0 ||
            rule.ParentProcessNameContains.Count > 0;

        if (!hasAnyCondition)
        {
            result.AddError("At least one detection condition is required.");
        }

        return result;
    }
}
