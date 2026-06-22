using PurpleTrace.Agent.Detection;
using PurpleTrace.Agent.Models;

namespace PurpleTrace.Agent.Tests;

public sealed class DetectionEngineTests
{
    [Fact]
    public void Analyze_ShouldGenerateAlert_WhenProcessAndCommandLineMatch()
    {
        var endpointEvent = new EndpointEvent
        {
            Source = "Sample",
            EventId = 1,
            EventType = EndpointEventType.ProcessCreate,
            ProcessName = "powershell.exe",
            CommandLine = "powershell.exe -NoProfile -ExecutionPolicy Bypass",
            ParentProcessName = "cmd.exe"
        };

        var rule = new DetectionRule
        {
            Id = "PT-RULE-001",
            Title = "Suspicious PowerShell Execution",
            Description = "PowerShell executed with suspicious command-line arguments.",
            Severity = "High",
            MitreTactic = "Execution",
            MitreTechniqueId = "T1059.001",
            MitreTechniqueName = "PowerShell",
            ProcessNameContains = new List<string>
            {
                "powershell.exe"
            },
            CommandLineContains = new List<string>
            {
                "-NoProfile"
            }
        };

        var engine = new DetectionEngine(new[] { rule });

        var alerts = engine.Analyze(endpointEvent);

        Assert.Single(alerts);
        Assert.Equal("PT-RULE-001", alerts[0].RuleId);
        Assert.Equal("Suspicious PowerShell Execution", alerts[0].RuleName);
        Assert.Equal("High", alerts[0].Severity);
        Assert.Equal("T1059.001", alerts[0].MitreTechniqueId);
    }

    [Fact]
    public void Analyze_ShouldNotGenerateAlert_WhenProcessDoesNotMatch()
    {
        var endpointEvent = new EndpointEvent
        {
            Source = "Sample",
            EventId = 1,
            EventType = EndpointEventType.ProcessCreate,
            ProcessName = "notepad.exe",
            CommandLine = "notepad.exe",
            ParentProcessName = "explorer.exe"
        };

        var rule = new DetectionRule
        {
            Id = "PT-RULE-001",
            Title = "Suspicious PowerShell Execution",
            Description = "PowerShell executed with suspicious command-line arguments.",
            Severity = "High",
            MitreTactic = "Execution",
            MitreTechniqueId = "T1059.001",
            MitreTechniqueName = "PowerShell",
            ProcessNameContains = new List<string>
            {
                "powershell.exe"
            },
            CommandLineContains = new List<string>
            {
                "-NoProfile"
            }
        };

        var engine = new DetectionEngine(new[] { rule });

        var alerts = engine.Analyze(endpointEvent);

        Assert.Empty(alerts);
    }

    [Fact]
    public void Analyze_ShouldGenerateAlert_WhenParentProcessMatches()
    {
        var endpointEvent = new EndpointEvent
        {
            Source = "Sample",
            EventId = 1,
            EventType = EndpointEventType.ProcessCreate,
            ProcessName = "powershell.exe",
            CommandLine = "powershell.exe",
            ParentProcessName = "cmd.exe"
        };

        var rule = new DetectionRule
        {
            Id = "PT-RULE-002",
            Title = "Command Shell Started PowerShell",
            Description = "PowerShell was launched from cmd.exe.",
            Severity = "Medium",
            MitreTactic = "Execution",
            MitreTechniqueId = "T1059",
            MitreTechniqueName = "Command and Scripting Interpreter",
            ProcessNameContains = new List<string>
            {
                "powershell.exe"
            },
            ParentProcessNameContains = new List<string>
            {
                "cmd.exe"
            }
        };

        var engine = new DetectionEngine(new[] { rule });

        var alerts = engine.Analyze(endpointEvent);

        Assert.Single(alerts);
        Assert.Equal("PT-RULE-002", alerts[0].RuleId);
        Assert.Equal("Medium", alerts[0].Severity);
    }
}
