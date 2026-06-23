using System.Text;
using PurpleTrace.Agent.Models;

namespace PurpleTrace.Agent.Exporters;

public sealed class InvestigationMarkdownExporter
{
    public void Export(string outputPath, List<DetectionAlert> alerts)
    {
        var directory = Path.GetDirectoryName(outputPath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var builder = new StringBuilder();

        builder.AppendLine("# PurpleTrace Investigation Report");
        builder.AppendLine();
        builder.AppendLine($"Generated UTC: {DateTime.UtcNow:O}");
        builder.AppendLine($"Total alerts: {alerts.Count}");
        builder.AppendLine();
        builder.AppendLine("---");
        builder.AppendLine();

        if (alerts.Count == 0)
        {
            builder.AppendLine("No alerts were exported for investigation.");
            File.WriteAllText(outputPath, builder.ToString());
            return;
        }

        for (var index = 0; index < alerts.Count; index++)
        {
            AppendAlertCase(builder, alerts[index], index + 1);
        }

        File.WriteAllText(outputPath, builder.ToString());
    }

    private static void AppendAlertCase(StringBuilder builder, DetectionAlert alert, int caseNumber)
    {
        builder.AppendLine($"## Case {caseNumber}: {ValueOrUnknown(alert.RuleName)}");
        builder.AppendLine();
        builder.AppendLine("### 1. Case Summary");
        builder.AppendLine();
        builder.AppendLine($"**Case ID:** PT-CASE-{caseNumber:000}");
        builder.AppendLine($"**Status:** Open");
        builder.AppendLine($"**Outcome:** Needs More Context");
        builder.AppendLine($"**Severity:** {ValueOrUnknown(alert.Severity)}");
        builder.AppendLine();

        builder.AppendLine("### 2. Alert Details");
        builder.AppendLine();
        builder.AppendLine($"**Alert ID:** {ValueOrUnknown(alert.AlertId)}");
        builder.AppendLine($"**Timestamp UTC:** {alert.TimestampUtc:O}");
        builder.AppendLine($"**Rule ID:** {ValueOrUnknown(alert.RuleId)}");
        builder.AppendLine($"**Rule Name:** {ValueOrUnknown(alert.RuleName)}");
        builder.AppendLine($"**Hostname:** {ValueOrUnknown(alert.Hostname)}");
        builder.AppendLine($"**Username:** {ValueOrUnknown(alert.UserName)}");
        builder.AppendLine();

        builder.AppendLine("### 3. MITRE ATT&CK Mapping");
        builder.AppendLine();
        builder.AppendLine($"**Tactic:** {ValueOrUnknown(alert.MitreTactic)}");
        builder.AppendLine($"**Technique ID:** {ValueOrUnknown(alert.MitreTechniqueId)}");
        builder.AppendLine($"**Technique Name:** {ValueOrUnknown(alert.MitreTechniqueName)}");
        builder.AppendLine();

        builder.AppendLine("### 4. Rule Context");
        builder.AppendLine();
        builder.AppendLine($"**Rule Author:** {ValueOrUnknown(alert.RuleAuthor)}");
        builder.AppendLine($"**Rule Created UTC:** {alert.RuleCreatedUtc:O}");
        builder.AppendLine();

        builder.AppendLine("**Rule Tags:**");
        AppendList(builder, alert.RuleTags);
        builder.AppendLine();

        builder.AppendLine("**Rule References:**");
        AppendList(builder, alert.RuleReferences);
        builder.AppendLine();

        builder.AppendLine("### 5. Process Evidence");
        builder.AppendLine();
        builder.AppendLine($"**Process Name:** {ValueOrUnknown(alert.ProcessName)}");
        builder.AppendLine($"**Process Path:** {ValueOrUnknown(alert.ProcessPath)}");
        builder.AppendLine();
        builder.AppendLine("**Command Line:**");
        AppendCodeBlock(builder, alert.CommandLine);
        builder.AppendLine();

        builder.AppendLine($"**Parent Process Name:** {ValueOrUnknown(alert.ParentProcessName)}");
        builder.AppendLine();
        builder.AppendLine("**Parent Command Line:**");
        AppendCodeBlock(builder, alert.ParentCommandLine);
        builder.AppendLine();

        builder.AppendLine("### 6. Matched Evidence");
        builder.AppendLine();
        builder.AppendLine("**Reason:**");
        builder.AppendLine();
        builder.AppendLine(ValueOrUnknown(alert.Reason));
        builder.AppendLine();

        builder.AppendLine("**Evidence Summary:**");
        builder.AppendLine();
        builder.AppendLine(ValueOrUnknown(alert.EvidenceSummary));
        builder.AppendLine();

        builder.AppendLine("**Matched Fields:**");
        AppendList(builder, alert.MatchedFields);
        builder.AppendLine();

        builder.AppendLine("**Matched Values:**");
        AppendList(builder, alert.MatchedValues);
        builder.AppendLine();

        builder.AppendLine("### 7. Source Event Review");
        builder.AppendLine();

        if (alert.SourceEvent is null)
        {
            builder.AppendLine("No source event was attached to this alert.");
            builder.AppendLine();
        }
        else
        {
            builder.AppendLine($"**Event Source:** {ValueOrUnknown(alert.SourceEvent.Source)}");
            builder.AppendLine($"**Event ID:** {alert.SourceEvent.EventId}");
            builder.AppendLine($"**Event Type:** {alert.SourceEvent.EventType}");
            builder.AppendLine($"**Process ID:** {alert.SourceEvent.ProcessId}");
            builder.AppendLine($"**Parent Process ID:** {alert.SourceEvent.ParentProcessId}");
            builder.AppendLine($"**Destination IP:** {ValueOrUnknown(alert.SourceEvent.DestinationIp)}");
            builder.AppendLine($"**Destination Port:** {alert.SourceEvent.DestinationPort}");
            builder.AppendLine($"**Protocol:** {ValueOrUnknown(alert.SourceEvent.Protocol)}");
            builder.AppendLine();
        }

        builder.AppendLine("### 8. Analyst Questions");
        builder.AppendLine();
        builder.AppendLine("- Was this command expected for the user?");
        builder.AppendLine("- Was the parent process normal?");
        builder.AppendLine("- Is the command line suspicious?");
        builder.AppendLine("- Does this match expected administrator behavior?");
        builder.AppendLine("- Are multiple rules firing on the same source event?");
        builder.AppendLine("- Is there repeated activity from the same host?");
        builder.AppendLine("- Is the MITRE mapping accurate?");
        builder.AppendLine("- Does the evidence summary explain the alert clearly?");
        builder.AppendLine();

        builder.AppendLine("### 9. Recommended Next Steps");
        builder.AppendLine();
        builder.AppendLine("- Review related alerts from the same host.");
        builder.AppendLine("- Review related alerts from the same user.");
        builder.AppendLine("- Check whether the command line is expected.");
        builder.AppendLine("- Compare the parent process with normal behavior.");
        builder.AppendLine("- Validate whether this is admin activity or suspicious execution.");
        builder.AppendLine("- Document the final decision.");
        builder.AppendLine();

        builder.AppendLine("### 10. Final Decision");
        builder.AppendLine();
        builder.AppendLine("**Classification:** Needs More Context");
        builder.AppendLine();
        builder.AppendLine("**Reason:**");
        builder.AppendLine();
        builder.AppendLine("The alert contains detection evidence, but additional host and user context should be reviewed before making a final decision.");
        builder.AppendLine();
        builder.AppendLine("---");
        builder.AppendLine();
    }

    private static void AppendList(StringBuilder builder, IEnumerable<string> values)
    {
        var written = false;

        foreach (var value in values.Where(value => !string.IsNullOrWhiteSpace(value)))
        {
            builder.AppendLine($"- {value}");
            written = true;
        }

        if (!written)
        {
            builder.AppendLine("- N/A");
        }
    }

    private static void AppendCodeBlock(StringBuilder builder, string value)
    {
        builder.AppendLine("```text");
        builder.AppendLine(ValueOrUnknown(value));
        builder.AppendLine("```");
    }

    private static string ValueOrUnknown(string value)
    {
        return string.IsNullOrWhiteSpace(value) ? "N/A" : value;
    }
}