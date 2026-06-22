using System.Text.Json;
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

var alert = new DetectionAlert
{
    RuleId = "PT-RULE-001",
    RuleName = "Suspicious PowerShell Execution",
    Severity = "High",
    MitreTactic = "Execution",
    MitreTechniqueId = "T1059.001",
    MitreTechniqueName = "PowerShell",
    Hostname = endpointEvent.Hostname,
    ProcessName = endpointEvent.ProcessName,
    CommandLine = endpointEvent.CommandLine,
    Reason = "PowerShell executed with suspicious command-line arguments.",
    SourceEvent = endpointEvent
};

var options = new JsonSerializerOptions
{
    WriteIndented = true
};

Console.WriteLine("PurpleTrace Agent - Endpoint Event Model Test");
Console.WriteLine();
Console.WriteLine(JsonSerializer.Serialize(alert, options));
