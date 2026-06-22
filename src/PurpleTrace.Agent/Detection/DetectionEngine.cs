using PurpleTrace.Agent.Models;

namespace PurpleTrace.Agent.Detection;

public sealed class DetectionEngine
{
    private readonly List<DetectionRule> _rules;

    public DetectionEngine(IEnumerable<DetectionRule> rules)
    {
        _rules = rules.ToList();
    }

    public List<DetectionAlert> Analyze(EndpointEvent endpointEvent)
    {
        var alerts = new List<DetectionAlert>();

        foreach (var rule in _rules)
        {
            if (IsMatch(rule, endpointEvent))
            {
                alerts.Add(CreateAlert(rule, endpointEvent));
            }
        }

        return alerts;
    }

    private static bool IsMatch(DetectionRule rule, EndpointEvent endpointEvent)
    {
        var processMatch = MatchesAny(endpointEvent.ProcessName, rule.ProcessNameContains);
        var commandLineMatch = MatchesAny(endpointEvent.CommandLine, rule.CommandLineContains);
        var parentProcessMatch = MatchesAny(endpointEvent.ParentProcessName, rule.ParentProcessNameContains);

        if (rule.ProcessNameContains.Count > 0 && !processMatch)
        {
            return false;
        }

        if (rule.CommandLineContains.Count > 0 && !commandLineMatch)
        {
            return false;
        }

        if (rule.ParentProcessNameContains.Count > 0 && !parentProcessMatch)
        {
            return false;
        }

        return true;
    }

    private static bool MatchesAny(string value, List<string> patterns)
    {
        if (patterns.Count == 0)
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return patterns.Any(pattern =>
            value.Contains(pattern, StringComparison.OrdinalIgnoreCase));
    }

    private static DetectionAlert CreateAlert(DetectionRule rule, EndpointEvent endpointEvent)
    {
        return new DetectionAlert
        {
            RuleId = rule.Id,
            RuleName = rule.Title,
            Severity = rule.Severity,
            MitreTactic = rule.MitreTactic,
            MitreTechniqueId = rule.MitreTechniqueId,
            MitreTechniqueName = rule.MitreTechniqueName,
            Hostname = endpointEvent.Hostname,
            ProcessName = endpointEvent.ProcessName,
            CommandLine = endpointEvent.CommandLine,
            Reason = rule.Description,
            SourceEvent = endpointEvent
        };
    }
}
