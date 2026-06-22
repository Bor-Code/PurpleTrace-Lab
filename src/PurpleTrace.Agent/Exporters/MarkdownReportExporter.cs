using System.Text;
using PurpleTrace.Agent.Models;

namespace PurpleTrace.Agent.Exporters;

public sealed class MarkdownReportExporter
{
    public void Export(string outputPath, IEnumerable<DetectionAlert> alerts)
    {
        var alertList = alerts.ToList();
        var directory = Path.GetDirectoryName(outputPath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var builder = new StringBuilder();

        builder.AppendLine("# PurpleTrace Detection Report");
        builder.AppendLine();
        builder.AppendLine($"Generated UTC: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
        builder.AppendLine($"Total Alerts: {alertList.Count}");
        builder.AppendLine();

        if (alertList.Count == 0)
        {
            builder.AppendLine("No alerts were generated.");
            File.WriteAllText(outputPath, builder.ToString());
            return;
        }

        AppendSeveritySummary(builder, alertList);
        AppendMitreSummary(builder, alertList);
        AppendAlertSummary(builder, alertList);
        AppendAlertDetails(builder, alertList);

        File.WriteAllText(outputPath, builder.ToString());
    }

    private static void AppendSeveritySummary(StringBuilder builder, List<DetectionAlert> alerts)
    {
        builder.AppendLine("## Severity Summary");
        builder.AppendLine();
        builder.AppendLine("| Severity | Count |");
        builder.AppendLine("|---|---|");

        var severityGroups = alerts
            .GroupBy(alert => alert.Severity)
            .OrderByDescending(group => GetSeverityRank(group.Key))
            .ThenBy(group => group.Key);

        foreach (var group in severityGroups)
        {
            builder.AppendLine($"| {group.Key} | {group.Count()} |");
        }

        builder.AppendLine();
    }

    private static void AppendMitreSummary(StringBuilder builder, List<DetectionAlert> alerts)
    {
        builder.AppendLine("## MITRE Technique Summary");
        builder.AppendLine();
        builder.AppendLine("| Technique ID | Technique Name | Count |");
        builder.AppendLine("|---|---|---|");

        var mitreGroups = alerts
            .GroupBy(alert => new
            {
                alert.MitreTechniqueId,
                alert.MitreTechniqueName
            })
            .OrderBy(group => group.Key.MitreTechniqueId);

        foreach (var group in mitreGroups)
        {
            builder.AppendLine($"| {group.Key.MitreTechniqueId} | {group.Key.MitreTechniqueName} | {group.Count()} |");
        }

        builder.AppendLine();
    }

    private static void AppendAlertSummary(StringBuilder builder, List<DetectionAlert> alerts)
    {
        builder.AppendLine("## Alert Summary");
        builder.AppendLine();
        builder.AppendLine("| Rule ID | Rule Name | Severity | MITRE Technique | Process |");
        builder.AppendLine("|---|---|---|---|---|");

        foreach (var alert in alerts.OrderByDescending(alert => GetSeverityRank(alert.Severity)).ThenBy(alert => alert.RuleId))
        {
            builder.AppendLine($"| {alert.RuleId} | {alert.RuleName} | {alert.Severity} | {alert.MitreTechniqueId} {alert.MitreTechniqueName} | {alert.ProcessName} |");
        }

        builder.AppendLine();
    }

    private static void AppendAlertDetails(StringBuilder builder, List<DetectionAlert> alerts)
    {
        builder.AppendLine("## Alert Details");
        builder.AppendLine();

        foreach (var alert in alerts.OrderByDescending(alert => GetSeverityRank(alert.Severity)).ThenBy(alert => alert.RuleId))
        {
            builder.AppendLine($"### {alert.RuleId} - {alert.RuleName}");
            builder.AppendLine();
            builder.AppendLine($"- Severity: {alert.Severity}");
            builder.AppendLine($"- MITRE Tactic: {alert.MitreTactic}");
            builder.AppendLine($"- MITRE Technique: {alert.MitreTechniqueId} - {alert.MitreTechniqueName}");
            if (!string.IsNullOrWhiteSpace(alert.RuleAuthor))
            {
                builder.AppendLine($"- Rule Author: {alert.RuleAuthor}");
            }

            if (!string.IsNullOrWhiteSpace(alert.RuleCreatedUtc))
            {
                builder.AppendLine($"- Rule Created UTC: {alert.RuleCreatedUtc}");
            }

            if (alert.RuleTags.Count > 0)
            {
                builder.AppendLine($"- Rule Tags: {string.Join(", ", alert.RuleTags)}");
            }

            if (alert.RuleReferences.Count > 0)
            {
                builder.AppendLine("- Rule References:");

                foreach (var reference in alert.RuleReferences)
                {
                    builder.AppendLine($"  - {reference}");
                }
            }
            builder.AppendLine($"- Hostname: {alert.Hostname}");
            builder.AppendLine($"- Process: {alert.ProcessName}");
            builder.AppendLine($"- Command Line: {alert.CommandLine}");
            builder.AppendLine($"- Reason: {alert.Reason}");
            builder.AppendLine();

            if (alert.SourceEvent is not null)
            {
                builder.AppendLine("#### Source Event");
                builder.AppendLine();
                builder.AppendLine($"- Source: {alert.SourceEvent.Source}");
                builder.AppendLine($"- Event ID: {alert.SourceEvent.EventId}");
                builder.AppendLine($"- User: {alert.SourceEvent.UserName}");
                builder.AppendLine($"- Parent Process: {alert.SourceEvent.ParentProcessName}");
                builder.AppendLine($"- Parent Command Line: {alert.SourceEvent.ParentCommandLine}");
                builder.AppendLine();
            }
        }
    }

    private static int GetSeverityRank(string severity)
    {
        return severity.ToLowerInvariant() switch
        {
            "critical" => 5,
            "high" => 4,
            "medium" => 3,
            "low" => 2,
            "informational" => 1,
            _ => 0
        };
    }
}
