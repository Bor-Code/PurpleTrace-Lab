using System.Text.Json;
using PurpleTrace.Agent;
using PurpleTrace.Agent.Detection;
using PurpleTrace.Agent.Exporters;
using PurpleTrace.Agent.Models;

var cliOptions = CliOptionsParser.Parse(args);

var rulesDirectory = ResolvePath(cliOptions.RulesDirectory);
var outputPath = ResolvePath(cliOptions.OutputPath);

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

var exporter = new JsonAlertExporter();
exporter.Export(outputPath, alerts);

var options = new JsonSerializerOptions
{
    WriteIndented = true
};

Console.WriteLine("PurpleTrace Agent - CLI Detection Pipeline Test");
Console.WriteLine();
Console.WriteLine($"Rules directory: {rulesDirectory}");
Console.WriteLine($"Output path: {outputPath}");
Console.WriteLine($"Loaded rules: {rules.Count}");
Console.WriteLine($"Generated alerts: {alerts.Count}");
Console.WriteLine();
Console.WriteLine(JsonSerializer.Serialize(alerts, options));

static string ResolvePath(string path)
{
    if (Path.IsPathRooted(path))
    {
        return path;
    }

    var currentDirectory = Directory.GetCurrentDirectory();

    while (!string.IsNullOrWhiteSpace(currentDirectory))
    {
        var candidate = Path.Combine(currentDirectory, path);
        var candidateDirectory = Path.GetDirectoryName(candidate);

        if (Directory.Exists(candidate) || File.Exists(candidate))
        {
            return candidate;
        }

        if (!string.IsNullOrWhiteSpace(candidateDirectory) && Directory.Exists(candidateDirectory))
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

    return Path.Combine(Directory.GetCurrentDirectory(), path);
}
