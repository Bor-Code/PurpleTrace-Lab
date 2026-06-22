using System.Text.Json;
using PurpleTrace.Agent;
using PurpleTrace.Agent.Collectors;
using PurpleTrace.Agent.Config;
using PurpleTrace.Agent.Detection;
using PurpleTrace.Agent.Exporters;
using PurpleTrace.Agent.Models;

var cliOptions = CliOptionsParser.Parse(args);

if (cliOptions.ShowHelp)
{
    CliHelp.Print();
    return;
}

ApplyConfigIfProvided(cliOptions);
CliOptionsParser.ApplyCommandLineOverrides(cliOptions, args);

if (!IsValidSource(cliOptions.Source))
{
    Console.WriteLine($"Invalid source: {cliOptions.Source}");
    Console.WriteLine("Supported sources: sample, sysmon");
    Console.WriteLine();
    CliHelp.Print();
    return;
}

if (!string.IsNullOrWhiteSpace(cliOptions.MinSeverity) && !SeverityRanker.IsValid(cliOptions.MinSeverity))
{
    Console.WriteLine($"Invalid minimum severity: {cliOptions.MinSeverity}");
    Console.WriteLine($"Valid values: {SeverityRanker.ValidValues}");
    Console.WriteLine();
    CliHelp.Print();
    return;
}

var rulesDirectory = ResolvePath(cliOptions.RulesDirectory);
var eventPathForSummary = ResolvePath(cliOptions.EventPath);
var outputPath = ResolvePath(cliOptions.OutputPath);
var reportPath = ResolvePath(cliOptions.ReportPath);
var csvPath = ResolvePath(cliOptions.CsvPath);
var htmlPath = ResolvePath(cliOptions.HtmlPath);
var summaryPath = ResolvePath(cliOptions.SummaryPath);
var ruleCoveragePath = ResolvePath(cliOptions.RuleCoveragePath);

var ruleLoader = new RuleLoader();
List<DetectionRule> rules;

try
{
    rules = ruleLoader.LoadFromDirectory(rulesDirectory);
}
catch (Exception exception)
{
    if (cliOptions.ValidateRules)
    {
        RuleValidationPrinter.PrintFailure(rulesDirectory, exception);
        Environment.ExitCode = 1;
        return;
    }

    Console.WriteLine("Could not load detection rules.");
    Console.WriteLine(exception.Message);
    Environment.ExitCode = 1;
    return;
}

if (cliOptions.ValidateRules)
{
    RuleValidationPrinter.PrintSuccess(rulesDirectory, rules);
    return;
}

if (cliOptions.ListRules)
{
    RuleCatalogPrinter.Print(rules);
    return;
}

if (cliOptions.ExportRuleCoverage)
{
    var ruleCoverageExporter = new RuleCoverageMarkdownExporter();
    ruleCoverageExporter.Export(ruleCoveragePath, rules);

    Console.WriteLine("PurpleTrace Rule Coverage Export");
    Console.WriteLine();
    Console.WriteLine($"Rules directory: {rulesDirectory}");
    Console.WriteLine($"Loaded rules: {rules.Count}");
    Console.WriteLine($"Rule coverage path: {ruleCoveragePath}");
    return;
}

var endpointEvents = LoadEndpointEvents(cliOptions);

var engine = new DetectionEngine(rules);
var detectedAlerts = new List<DetectionAlert>();

foreach (var endpointEvent in endpointEvents)
{
    detectedAlerts.AddRange(engine.Analyze(endpointEvent));
}

var severityFilteredAlerts = AlertSeverityFilter.FilterByMinimumSeverity(detectedAlerts, cliOptions.MinSeverity);
var mitreFilteredAlerts = AlertMitreTechniqueFilter.FilterByTechniqueId(severityFilteredAlerts, cliOptions.MitreTechniqueId);
var ruleFilteredAlerts = AlertRuleIdFilter.FilterByRuleId(mitreFilteredAlerts, cliOptions.RuleId);
var alerts = AlertTagFilter.FilterByTag(ruleFilteredAlerts, cliOptions.Tag);

var jsonExporter = new JsonAlertExporter();
jsonExporter.Export(outputPath, alerts);

var markdownExporter = new MarkdownReportExporter();
markdownExporter.Export(reportPath, alerts);

var csvExporter = new CsvAlertExporter();
csvExporter.Export(csvPath, alerts);

var htmlExporter = new HtmlReportExporter();
htmlExporter.Export(htmlPath, alerts);

var runSummary = BuildRunSummary(
    cliOptions,
    rulesDirectory,
    eventPathForSummary,
    outputPath,
    reportPath,
    csvPath,
    htmlPath,
    summaryPath,
    rules.Count,
    endpointEvents.Count,
    detectedAlerts.Count,
    alerts);

var runSummaryExporter = new RunSummaryExporter();
runSummaryExporter.Export(summaryPath, runSummary);

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
Console.WriteLine($"CSV output path: {csvPath}");
Console.WriteLine($"HTML report path: {htmlPath}");
Console.WriteLine($"Summary output path: {summaryPath}");
Console.WriteLine($"Loaded rules: {rules.Count}");
Console.WriteLine($"Loaded events: {endpointEvents.Count}");
Console.WriteLine($"Detected alerts before filtering: {detectedAlerts.Count}");

if (!string.IsNullOrWhiteSpace(cliOptions.MinSeverity))
{
    Console.WriteLine($"Minimum severity filter: {cliOptions.MinSeverity}");
}

if (!string.IsNullOrWhiteSpace(cliOptions.MitreTechniqueId))
{
    Console.WriteLine($"MITRE technique filter: {cliOptions.MitreTechniqueId}");
}

if (!string.IsNullOrWhiteSpace(cliOptions.RuleId))
{
    Console.WriteLine($"Rule ID filter: {cliOptions.RuleId}");
}

if (!string.IsNullOrWhiteSpace(cliOptions.Tag))
{
    Console.WriteLine($"Tag filter: {cliOptions.Tag}");
}

Console.WriteLine($"Exported alerts: {alerts.Count}");
Console.WriteLine();
Console.WriteLine(JsonSerializer.Serialize(alerts, options));

static void ApplyConfigIfProvided(CliOptions cliOptions)
{
    if (string.IsNullOrWhiteSpace(cliOptions.ConfigPath))
    {
        return;
    }

    var configPath = ResolvePath(cliOptions.ConfigPath);
    var loader = new AgentConfigLoader();
    var config = loader.Load(configPath);

    cliOptions.RulesDirectory = config.RulesDirectory;
    cliOptions.EventPath = config.EventPath;
    cliOptions.OutputPath = config.OutputPath;
    cliOptions.ReportPath = config.ReportPath;
    cliOptions.CsvPath = config.CsvPath;
    cliOptions.HtmlPath = config.HtmlPath;
    cliOptions.SummaryPath = config.SummaryPath;
    cliOptions.Source = config.Source;
    cliOptions.MinSeverity = config.MinSeverity;
    cliOptions.MitreTechniqueId = config.MitreTechniqueId;
    cliOptions.RuleId = config.RuleId;
    cliOptions.Tag = config.Tag;
    cliOptions.MaxEvents = config.MaxEvents;
}

static bool IsValidSource(string source)
{
    return source.Equals("sample", StringComparison.OrdinalIgnoreCase) ||
           source.Equals("sysmon", StringComparison.OrdinalIgnoreCase);
}

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

    return loader.LoadManyFromJsonFile(eventPath);
}

static DetectionRunSummary BuildRunSummary(
    CliOptions cliOptions,
    string rulesDirectory,
    string eventPath,
    string outputPath,
    string reportPath,
    string csvPath,
    string htmlPath,
    string summaryPath,
    int loadedRules,
    int loadedEvents,
    int detectedAlertsBeforeFiltering,
    List<DetectionAlert> exportedAlerts)
{
    var summary = new DetectionRunSummary
    {
        Source = cliOptions.Source,
        RulesDirectory = rulesDirectory,
        EventPath = eventPath,
        JsonOutputPath = outputPath,
        MarkdownReportPath = reportPath,
        CsvOutputPath = csvPath,
        HtmlReportPath = htmlPath,
        SummaryOutputPath = summaryPath,
        LoadedRules = loadedRules,
        LoadedEvents = loadedEvents,
        DetectedAlertsBeforeFiltering = detectedAlertsBeforeFiltering,
        ExportedAlerts = exportedAlerts.Count,
        MinSeverity = cliOptions.MinSeverity,
        MitreTechniqueId = cliOptions.MitreTechniqueId,
        RuleId = cliOptions.RuleId,
        Tag = cliOptions.Tag,
        SeverityCounts = CountBy(exportedAlerts, alert => alert.Severity),
        MitreTechniqueCounts = CountBy(exportedAlerts, alert => alert.MitreTechniqueId),
        RuleCounts = CountBy(exportedAlerts, alert => alert.RuleId)
    };

    if (!string.IsNullOrWhiteSpace(cliOptions.MinSeverity))
    {
        summary.ActiveFilters.Add($"min-severity={cliOptions.MinSeverity}");
    }

    if (!string.IsNullOrWhiteSpace(cliOptions.MitreTechniqueId))
    {
        summary.ActiveFilters.Add($"mitre-technique={cliOptions.MitreTechniqueId}");
    }

    if (!string.IsNullOrWhiteSpace(cliOptions.RuleId))
    {
        summary.ActiveFilters.Add($"rule-id={cliOptions.RuleId}");
    }

    if (!string.IsNullOrWhiteSpace(cliOptions.Tag))
    {
        summary.ActiveFilters.Add($"tag={cliOptions.Tag}");
    }

    return summary;
}

static Dictionary<string, int> CountBy(IEnumerable<DetectionAlert> alerts, Func<DetectionAlert, string> selector)
{
    return alerts
        .Select(selector)
        .Where(value => !string.IsNullOrWhiteSpace(value))
        .GroupBy(value => value, StringComparer.OrdinalIgnoreCase)
        .OrderBy(group => group.Key)
        .ToDictionary(group => group.Key, group => group.Count(), StringComparer.OrdinalIgnoreCase);
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
