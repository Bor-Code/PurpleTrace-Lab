using System.Text.Json;
using PurpleTrace.Agent;
using PurpleTrace.Agent.Collectors;
using PurpleTrace.Agent.Detection;
using PurpleTrace.Agent.Exporters;

var cliOptions = CliOptionsParser.Parse(args);

var rulesDirectory = ResolvePath(cliOptions.RulesDirectory);
var eventPath = ResolvePath(cliOptions.EventPath);
var outputPath = ResolvePath(cliOptions.OutputPath);

var ruleLoader = new RuleLoader();
var rules = ruleLoader.LoadFromDirectory(rulesDirectory);

var eventLoader = new EndpointEventLoader();
var endpointEvent = eventLoader.LoadFromJsonFile(eventPath);

var engine = new DetectionEngine(rules);
var alerts = engine.Analyze(endpointEvent);

var exporter = new JsonAlertExporter();
exporter.Export(outputPath, alerts);

var options = new JsonSerializerOptions
{
    WriteIndented = true
};

Console.WriteLine("PurpleTrace Agent - Sample Event Detection Pipeline");
Console.WriteLine();
Console.WriteLine($"Rules directory: {rulesDirectory}");
Console.WriteLine($"Event path: {eventPath}");
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
