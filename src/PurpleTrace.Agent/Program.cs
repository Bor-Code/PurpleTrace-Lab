using System.Text.Json;
using PurpleTrace.Agent.Detection;
using PurpleTrace.Agent.Models;

var rulesDirectory = ResolveRulesDirectory();

var loader = new RuleLoader();
var rules = loader.LoadFromDirectory(rulesDirectory);

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

var engine = new DetectionEngine(rules);
var alerts = engine.Analyze(endpointEvent);

var options = new JsonSerializerOptions
{
    WriteIndented = true
};

Console.WriteLine("PurpleTrace Agent - JSON Rule Loader Test");
Console.WriteLine();
Console.WriteLine($"Loaded rules: {rules.Count}");
Console.WriteLine($"Generated alerts: {alerts.Count}");
Console.WriteLine();
Console.WriteLine(JsonSerializer.Serialize(alerts, options));

static string ResolveRulesDirectory()
{
    var currentDirectory = Directory.GetCurrentDirectory();

    while (!string.IsNullOrWhiteSpace(currentDirectory))
    {
        var candidate = Path.Combine(currentDirectory, "rules");

        if (Directory.Exists(candidate))
        {
            return candidate;
        }

        var parent = Directory.GetParent(currentDirectory);

        if (parent is null)
        {
            break;
        }

        currentDirectory = parent.FullName;
    }

    throw new DirectoryNotFoundException("Could not locate the rules directory.");
}
