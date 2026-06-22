namespace PurpleTrace.Agent.Models;

public sealed class DetectionRunSummary
{
    public string RunId { get; set; } = Guid.NewGuid().ToString("N");
    public DateTime GeneratedUtc { get; set; } = DateTime.UtcNow;

    public string Source { get; set; } = string.Empty;
    public string RulesDirectory { get; set; } = string.Empty;
    public string EventPath { get; set; } = string.Empty;

    public string JsonOutputPath { get; set; } = string.Empty;
    public string MarkdownReportPath { get; set; } = string.Empty;
    public string CsvOutputPath { get; set; } = string.Empty;
    public string HtmlReportPath { get; set; } = string.Empty;
    public string SummaryOutputPath { get; set; } = string.Empty;

    public int LoadedRules { get; set; }
    public int LoadedEvents { get; set; }
    public int DetectedAlertsBeforeFiltering { get; set; }
    public int ExportedAlerts { get; set; }

    public string MinSeverity { get; set; } = string.Empty;
    public string MitreTechniqueId { get; set; } = string.Empty;
    public string RuleId { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;

    public List<string> ActiveFilters { get; set; } = new();

    public Dictionary<string, int> SeverityCounts { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, int> MitreTechniqueCounts { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, int> RuleCounts { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
