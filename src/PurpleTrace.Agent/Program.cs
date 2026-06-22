using System.Text.Json;
using PurpleTrace.Agent.Detection;
using PurpleTrace.Agent.Models;

var endpointEvent = new EndpointEvent
{
    Source = "Sysmon",
    EventId = 1,
    EventType = EndpointEventType.ProcessCreate,
    UserName = Environment.UserName,
    ProcessId = 4321,
    ProcessName = "powershell.exe",
    ProcessPath = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe",
    CommandLine = "powershell.exe -NoProfile -ExecutionPolicy Bypass",
    ParentProcessId = 1234,
    ParentProcessName = "cmd.exe",
    ParentProcessPath = @"C:\Windows\System32\cmd.exe",
    ParentCommandLine = "cmd.exe"
};

var rules = new List<DetectionRule>
{
    new DetectionRule
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
            "powershell.exe",
            "pwsh.exe"
        },
        CommandLineContains = new List<string>
        {
            "-NoProfile",
            "ExecutionPolicy Bypass",
            "-enc",
            "-nop"
        }
    },

    new DetectionRule
    {
        Id = "PT-RULE-002",
        Title = "Command Shell Started PowerShell",
        Description = "PowerShell was launched from cmd.exe, which may indicate scripted execution.",
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
    }
};

var engine = new DetectionEngine(rules);
var alerts = engine.Analyze(endpointEvent);

var options = new JsonSerializerOptions
{
    WriteIndented = true
};

Console.WriteLine("PurpleTrace Agent - Detection Engine Test");
Console.WriteLine();
Console.WriteLine(JsonSerializer.Serialize(alerts, options));
