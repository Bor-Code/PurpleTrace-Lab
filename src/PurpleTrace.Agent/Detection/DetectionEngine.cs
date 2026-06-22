using PurpleTrace.Agent.Models;

namespace PurpleTrace.Agent.Detection;

public sealed class DetectionEngine
{
    private readonly List<DetectionRule> rules;

    public DetectionEngine(IEnumerable<DetectionRule> rules)
    {
        this.rules = rules.ToList();
    }

    public List<DetectionAlert> Analyze(EndpointEvent endpointEvent)
    {
        var alerts = new List<DetectionAlert>();

        foreach (var rule in rules)
        {
            var matchedFields = new List<string>();
            var matchedValues = new List<string>();

            if (!MatchesRule(rule, endpointEvent, matchedFields, matchedValues))
            {
                continue;
            }

            alerts.Add(new DetectionAlert
            {
                RuleId = rule.Id,
                RuleName = rule.Title,
                Severity = rule.Severity,
                MitreTactic = rule.MitreTactic,
                MitreTechniqueId = rule.MitreTechniqueId,
                MitreTechniqueName = rule.MitreTechniqueName,

                RuleAuthor = rule.Author,
                RuleCreatedUtc = rule.CreatedUtc,
                RuleTags = rule.Tags.ToList(),
                RuleReferences = rule.References.ToList(),

                Hostname = endpointEvent.Hostname,
                UserName = endpointEvent.UserName,
                ProcessName = endpointEvent.ProcessName,
                ProcessPath = endpointEvent.ProcessPath,
                CommandLine = endpointEvent.CommandLine,
                ParentProcessName = endpointEvent.ParentProcessName,
                ParentCommandLine = endpointEvent.ParentCommandLine,

                Reason = rule.Description,
                EvidenceSummary = string.Join("; ", matchedValues),
                MatchedFields = matchedFields.Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
                MatchedValues = matchedValues,

                SourceEvent = endpointEvent
            });
        }

        return alerts;
    }

    private static bool MatchesRule(
        DetectionRule rule,
        EndpointEvent endpointEvent,
        List<string> matchedFields,
        List<string> matchedValues)
    {
        return MatchesAny(
                   endpointEvent.ProcessName,
                   rule.ProcessNameContains,
                   "ProcessName",
                   matchedFields,
                   matchedValues) &&
               MatchesAny(
                   endpointEvent.CommandLine,
                   rule.CommandLineContains,
                   "CommandLine",
                   matchedFields,
                   matchedValues) &&
               MatchesAny(
                   endpointEvent.ParentProcessName,
                   rule.ParentProcessNameContains,
                   "ParentProcessName",
                   matchedFields,
                   matchedValues);
    }

    private static bool MatchesAny(
        string fieldValue,
        List<string> patterns,
        string fieldName,
        List<string> matchedFields,
        List<string> matchedValues)
    {
        if (patterns.Count == 0)
        {
            return true;
        }

        var matched = false;

        foreach (var pattern in patterns)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                continue;
            }

            if (!fieldValue.Contains(pattern, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            matched = true;
            matchedFields.Add(fieldName);
            matchedValues.Add($"{fieldName} contains {pattern}");
        }

        return matched;
    }
}
