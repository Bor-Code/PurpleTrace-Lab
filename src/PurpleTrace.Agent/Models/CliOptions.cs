namespace PurpleTrace.Agent.Models;

public sealed class CliOptions
{
    public string ConfigPath { get; set; } = string.Empty;
    public string RulesDirectory { get; set; } = "rules";
    public string EventPath { get; set; } = "samples/sample-powershell-event.json";
    public string OutputPath { get; set; } = "samples/latest-alerts.local.json";
    public string ReportPath { get; set; } = "samples/latest-report.local.md";
    public string CsvPath { get; set; } = "samples/latest-alerts.local.csv";
    public string HtmlPath { get; set; } = "samples/latest-report.local.html";
    public string Source { get; set; } = "sample";
    public string MinSeverity { get; set; } = string.Empty;
    public string MitreTechniqueId { get; set; } = string.Empty;
    public string RuleId { get; set; } = string.Empty;
    public int MaxEvents { get; set; } = 20;
    public bool ShowHelp { get; set; }
    public bool ListRules { get; set; }
    public bool ValidateRules { get; set; }
}
