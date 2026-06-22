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

        builder.AppendLine("## Alert Summary");
        builder.AppendLine();
        builder.AppendLine("| Rule ID | Rule Name | Severity | MITRE Technique | Process |");
        builder.AppendLine("|---|---|---|---|---|");

        foreach (var alert in alertList)
        {
            builder.AppendLine($"| {alert.RuleId} | {alert.RuleName} | {alert.Severity} | {alert.MitreTechniqueId} {alert.MitreTechniqueName} | {alert.ProcessName} |");
        }

        builder.AppendLine();
        builder.AppendLine("## Alert Details");
        builder.AppendLine();

        foreach (var alert in alertList)
        {
            builder.AppendLine($"### {alert.RuleId} - {alert.RuleName}");
            builder.AppendLine();
            builder.AppendLine($"- Severity: {alert.Severity}");
            builder.AppendLine($"- MITRE Tactic: {alert.MitreTactic}");
            builder.AppendLine($"- MITRE Technique: {alert.MitreTechniqueId} - {alert.MitreTechniqueName}");
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

        File.WriteAllText(outputPath, builder.ToString());
    }
}
