using PurpleTrace.Agent.Detection;
using PurpleTrace.Agent.Models;

namespace PurpleTrace.Agent.Tests;

public sealed class AlertMetadataEnrichmentTests
{
    [Fact]
    public void Analyze_ShouldIncludeRuleMetadata_WhenAlertIsGenerated()
    {
        var endpointEvent = new EndpointEvent
        {
            Source = "Sample",
            EventId = 1,
            EventType = EndpointEventType.ProcessCreate,
            Hostname = "BORAN",
            ProcessName = "powershell.exe",
            CommandLine = "powershell.exe -NoProfile",
            ParentProcessName = "cmd.exe"
        };

        var rule = new DetectionRule
        {
            Id = "PT-RULE-TEST",
            Title = "Test Metadata Rule",
            Description = "Test rule metadata enrichment.",
            Severity = "High",
            MitreTactic = "Execution",
            MitreTechniqueId = "T1059.001",
            MitreTechniqueName = "PowerShell",
            Author = "PurpleTrace Lab",
            CreatedUtc = "2026-06-22T00:00:00Z",
            Tags = new List<string>
            {
                "powershell",
                "metadata"
            },
            References = new List<string>
            {
                "MITRE ATT&CK T1059.001 - PowerShell"
            },
            ProcessNameContains = new List<string>
            {
                "powershell.exe"
            }
        };

        var engine = new DetectionEngine(new[] { rule });

        var alerts = engine.Analyze(endpointEvent);

        Assert.Single(alerts);
        Assert.Equal("PurpleTrace Lab", alerts[0].RuleAuthor);
        Assert.Equal("2026-06-22T00:00:00Z", alerts[0].RuleCreatedUtc);
        Assert.Contains("powershell", alerts[0].RuleTags);
        Assert.Contains("metadata", alerts[0].RuleTags);
        Assert.Contains("MITRE ATT&CK T1059.001 - PowerShell", alerts[0].RuleReferences);
    }
}
