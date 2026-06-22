using System.Text;
using PurpleTrace.Agent.Models;

namespace PurpleTrace.Agent.Exporters;

public sealed class CsvAlertExporter
{
    public void Export(string outputPath, IEnumerable<DetectionAlert> alerts)
    {
        var directory = Path.GetDirectoryName(outputPath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var builder = new StringBuilder();

        builder.AppendLine("TimestampUtc,RuleId,RuleName,Severity,MitreTactic,MitreTechniqueId,MitreTechniqueName,RuleAuthor,RuleCreatedUtc,RuleTags,RuleReferences,EvidenceSummary,MatchedFields,MatchedValues,Hostname,UserName,ProcessName,CommandLine,ParentProcessName,Reason");

        foreach (var alert in alerts)
        {
            builder.AppendLine(string.Join(",",
                Escape(alert.TimestampUtc.ToString("O")),
                Escape(alert.RuleId),
                Escape(alert.RuleName),
                Escape(alert.Severity),
                Escape(alert.MitreTactic),
                Escape(alert.MitreTechniqueId),
                Escape(alert.MitreTechniqueName),
                Escape(alert.RuleAuthor),
                Escape(alert.RuleCreatedUtc),
                Escape(string.Join(";", alert.RuleTags)),
                Escape(string.Join(";", alert.RuleReferences)),
                Escape(alert.EvidenceSummary),
                Escape(string.Join(";", alert.MatchedFields)),
                Escape(string.Join(";", alert.MatchedValues)),
                Escape(alert.Hostname),
                Escape(alert.UserName),
                Escape(alert.ProcessName),
                Escape(alert.CommandLine),
                Escape(alert.ParentProcessName),
                Escape(alert.Reason)
            ));
        }

        File.WriteAllText(outputPath, builder.ToString());
    }

    private static string Escape(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var escaped = value.Replace("\"", "\"\"");

        if (escaped.Contains(',') || escaped.Contains('"') || escaped.Contains('\n') || escaped.Contains('\r'))
        {
            return $"\"{escaped}\"";
        }

        return escaped;
    }
}
