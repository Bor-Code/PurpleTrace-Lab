using System.Text.Json;
using PurpleTrace.Agent;
using PurpleTrace.Agent.Collectors;
using PurpleTrace.Agent.Detection;
using PurpleTrace.Agent.Exporters;
using PurpleTrace.Agent.Models;

var cliOptions = CliOptionsParser.Parse(args);

var rulesDirectory = ResolvePath(cliOptions.RulesDirectory);
var outputPath = ResolvePath(cliOptions.OutputPath);
var reportPath = ResolvePath(cliOptions.ReportPath);

var ruleLoader = new RuleLoader();
var rules = ruleLoader.LoadFromDirectory(rulesDirectory);

var endpointEvents = LoadEndpointEvents(cliOptions);

var engine = new DetectionEngine(rules);
var alerts = new List<DetectionAlert>();

foreach (var endpointEvent in endpointEvents)
{
    alerts.AddRange(engine.Analyze(endpointEvent));
}

var jsonExporter = new JsonAlertExporter();
jsonExporter.Export(outputPath, alerts);

var markdownExporter = new MarkdownReportExporter();
markdownExporter.Export(reportPath, alerts);

var options = new JsonSerializerOptions
{
    WriteIndented = true
};

Console.WriteLine("PurpleTrace Agent - Detection Pipeline");
Console.WriteLine();
Console.WriteLine($"Source: {cliOptions.Source}");
Console.WriteLine($"Rules directory: {rulesDirectory}");
Console.WriteLine($"JSON output path: {outputPath}");
Console.WriteLine($"Markdown report path: {reportPath}");
Console.WriteLine($"Loaded rules: {rules.Count}");
Console.WriteLine($"Loaded events: {endpointEvents.Count}");
Console.WriteLine($"Generated alerts: {alerts.Count}");
Console.WriteLine();
Console.WriteLine(JsonSerializer.Serialize(alerts, options));

static List<EndpointEvent> LoadEndpointEvents(CliOptions cliOptions)
{
    if (cliOptions.Source.Equals("sysmon", StringComparison.OrdinalIgnoreCase))
    {
        try
        {
            var collector = new SysmonProcessCollector();
            return collector.GetRecentProcessCreateEvents(cliOptions.MaxEvents);
        }
        catch (Exception exception)
        {
            Console.WriteLine("Could not read Sysmon events.");
            Console.WriteLine("Make sure Sysmon is installed and the Microsoft-Windows-Sysmon/Operational log exists.");
            Console.WriteLine($"Error: {exception.Message}");
            return new List<EndpointEvent>();
        }
    }

    var eventPath = ResolvePath(cliOptions.EventPath);
    var loader = new EndpointEventLoader();

    return new List<EndpointEvent>
    {
        loader.LoadFromJsonFile(eventPath)
    };
}

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
