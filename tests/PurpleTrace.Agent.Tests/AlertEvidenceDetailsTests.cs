using PurpleTrace.Agent.Detection;
using PurpleTrace.Agent.Models;

namespace PurpleTrace.Agent.Tests;

public sealed class AlertEvidenceDetailsTests
{
    [Fact]
    public void Analyze_ShouldIncludeEvidenceDetails_WhenRuleMatches()
    {
        var endpointEvent = new EndpointEvent
        {
            Source = "Sample",
            EventId = 1,
            EventType = EndpointEventType.ProcessCreate,
            Hostname = "BORAN",
            UserName = "nonmr",
            ProcessName = "powershell.exe",
            CommandLine = "powershell.exe -NoProfile -ExecutionPolicy Bypass",
            ParentProcessName = "cmd.exe"
        };

        var rule = new DetectionRule
        {
            Id = "PT-RULE-TEST",
            Title = "Test Evidence Rule",
            Description = "Test evidence details.",
            Severity = "High",
            MitreTactic = "Execution",
            MitreTechniqueId = "T1059.001",
            MitreTechniqueName = "PowerShell",
            Author = "PurpleTrace Lab",
            CreatedUtc = "2026-06-22T00:00:00Z",
            Tags = new List<string>
            {
                "powershell",
                "evidence"
            },
            References = new List<string>
            {
                "MITRE ATT&CK T1059.001 - PowerShell"
            },
            ProcessNameContains = new List<string>
            {
                "powershell.exe"
            },
            CommandLineContains = new List<string>
            {
                "-NoProfile",
                "ExecutionPolicy Bypass"
            },
            ParentProcessNameContains = new List<string>
            {
                "cmd.exe"
            }
        };

        var engine = new DetectionEngine(new[] { rule });

        var alerts = engine.Analyze(endpointEvent);

        Assert.Single(alerts);
        Assert.Contains("ProcessName", alerts[0].MatchedFields);
        Assert.Contains("CommandLine", alerts[0].MatchedFields);
        Assert.Contains("ParentProcessName", alerts[0].MatchedFields);
        Assert.Contains("ProcessName contains powershell.exe", alerts[0].MatchedValues);
        Assert.Contains("CommandLine contains -NoProfile", alerts[0].MatchedValues);
        Assert.Contains("ParentProcessName contains cmd.exe", alerts[0].MatchedValues);
        Assert.Contains("powershell.exe", alerts[0].EvidenceSummary);
    }
}
