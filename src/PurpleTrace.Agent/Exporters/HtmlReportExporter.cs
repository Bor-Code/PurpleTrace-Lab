using System.Net;
using System.Text;
using PurpleTrace.Agent.Detection;
using PurpleTrace.Agent.Models;

namespace PurpleTrace.Agent.Exporters;

public sealed class HtmlReportExporter
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

        builder.AppendLine("<!DOCTYPE html>");
        builder.AppendLine("<html lang=\"en\">");
        builder.AppendLine("<head>");
        builder.AppendLine("  <meta charset=\"UTF-8\">");
        builder.AppendLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        builder.AppendLine("  <title>PurpleTrace Detection Report</title>");
        builder.AppendLine("  <style>");
        builder.AppendLine("    body { margin: 0; font-family: Arial, sans-serif; background: #0f172a; color: #e5e7eb; }");
        builder.AppendLine("    .container { max-width: 1100px; margin: 0 auto; padding: 32px; }");
        builder.AppendLine("    .header, .card, .alert { background: #111827; border: 1px solid #334155; border-radius: 16px; padding: 20px; margin-bottom: 18px; }");
        builder.AppendLine("    h1 { color: #c084fc; margin-bottom: 8px; }");
        builder.AppendLine("    h2 { color: #93c5fd; margin-top: 32px; }");
        builder.AppendLine("    table { width: 100%; border-collapse: collapse; background: #111827; border: 1px solid #334155; margin-bottom: 22px; }");
        builder.AppendLine("    th, td { padding: 12px; border-bottom: 1px solid #334155; text-align: left; vertical-align: top; }");
        builder.AppendLine("    th { color: #c4b5fd; background: #1e293b; }");
        builder.AppendLine("    .grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); gap: 16px; margin-top: 18px; }");
        builder.AppendLine("    .metric { font-size: 32px; font-weight: bold; }");
        builder.AppendLine("    .label { color: #94a3b8; }");
        builder.AppendLine("    .badge { display: inline-block; padding: 4px 10px; border-radius: 999px; background: #312e81; color: #ddd6fe; margin: 3px; font-size: 13px; }");
        builder.AppendLine("    code { background: #020617; border: 1px solid #334155; padding: 4px 7px; border-radius: 6px; color: #bfdbfe; }");
        builder.AppendLine("    .muted { color: #94a3b8; }");
        builder.AppendLine("  </style>");
        builder.AppendLine("</head>");
        builder.AppendLine("<body>");
        builder.AppendLine("<div class=\"container\">");

        builder.AppendLine("<div class=\"header\">");
        builder.AppendLine("<h1>PurpleTrace Detection Report</h1>");
        builder.AppendLine($"<div class=\"muted\">Generated UTC: {Encode(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))}</div>");
        builder.AppendLine("<div class=\"grid\">");
        builder.AppendLine($"<div class=\"card\"><div class=\"metric\">{alertList.Count}</div><div class=\"label\">Total Alerts</div></div>");
        builder.AppendLine($"<div class=\"card\"><div class=\"metric\">{CountBySeverity(alertList, "High")}</div><div class=\"label\">High Alerts</div></div>");
        builder.AppendLine($"<div class=\"card\"><div class=\"metric\">{CountBySeverity(alertList, "Medium")}</div><div class=\"label\">Medium Alerts</div></div>");
        builder.AppendLine($"<div class=\"card\"><div class=\"metric\">{alertList.Select(alert => alert.MitreTechniqueId).Distinct().Count()}</div><div class=\"label\">MITRE Techniques</div></div>");
        builder.AppendLine("</div>");
        builder.AppendLine("</div>");

        AppendSeveritySummary(builder, alertList);
        AppendMitreSummary(builder, alertList);
        AppendAlertSummary(builder, alertList);
        AppendAlertDetails(builder, alertList);

        builder.AppendLine("</div>");
        builder.AppendLine("</body>");
        builder.AppendLine("</html>");

        File.WriteAllText(outputPath, builder.ToString());
    }

    private static void AppendSeveritySummary(StringBuilder builder, List<DetectionAlert> alerts)
    {
        builder.AppendLine("<h2>Severity Summary</h2>");
        builder.AppendLine("<table>");
        builder.AppendLine("<tr><th>Severity</th><th>Count</th></tr>");

        var groups = alerts
            .GroupBy(alert => alert.Severity)
            .OrderByDescending(group => SeverityRanker.GetRank(group.Key));

        foreach (var group in groups)
        {
            builder.AppendLine($"<tr><td>{Encode(group.Key)}</td><td>{group.Count()}</td></tr>");
        }

        builder.AppendLine("</table>");
    }

    private static void AppendMitreSummary(StringBuilder builder, List<DetectionAlert> alerts)
    {
        builder.AppendLine("<h2>MITRE Technique Summary</h2>");
        builder.AppendLine("<table>");
        builder.AppendLine("<tr><th>Technique ID</th><th>Technique Name</th><th>Count</th></tr>");

        var groups = alerts
            .GroupBy(alert => new
            {
                alert.MitreTechniqueId,
                alert.MitreTechniqueName
            })
            .OrderBy(group => group.Key.MitreTechniqueId);

        foreach (var group in groups)
        {
            builder.AppendLine($"<tr><td>{Encode(group.Key.MitreTechniqueId)}</td><td>{Encode(group.Key.MitreTechniqueName)}</td><td>{group.Count()}</td></tr>");
        }

        builder.AppendLine("</table>");
    }

    private static void AppendAlertSummary(StringBuilder builder, List<DetectionAlert> alerts)
    {
        builder.AppendLine("<h2>Alert Summary</h2>");
        builder.AppendLine("<table>");
        builder.AppendLine("<tr><th>Rule</th><th>Severity</th><th>MITRE</th><th>Process</th></tr>");

        foreach (var alert in alerts.OrderByDescending(alert => SeverityRanker.GetRank(alert.Severity)).ThenBy(alert => alert.RuleId))
        {
            builder.AppendLine($"<tr><td>{Encode(alert.RuleId)} - {Encode(alert.RuleName)}</td><td>{Encode(alert.Severity)}</td><td>{Encode(alert.MitreTechniqueId)}</td><td>{Encode(alert.ProcessName)}</td></tr>");
        }

        builder.AppendLine("</table>");
    }

    private static void AppendAlertDetails(StringBuilder builder, List<DetectionAlert> alerts)
    {
        builder.AppendLine("<h2>Alert Details</h2>");

        foreach (var alert in alerts.OrderByDescending(alert => SeverityRanker.GetRank(alert.Severity)).ThenBy(alert => alert.RuleId))
        {
            builder.AppendLine("<div class=\"alert\">");
            builder.AppendLine($"<h3>{Encode(alert.RuleId)} - {Encode(alert.RuleName)}</h3>");
            builder.AppendLine($"<p><strong>Severity:</strong> {Encode(alert.Severity)}</p>");
            builder.AppendLine($"<p><strong>MITRE:</strong> {Encode(alert.MitreTechniqueId)} - {Encode(alert.MitreTechniqueName)}</p>");
            builder.AppendLine($"<p><strong>Hostname:</strong> {Encode(alert.Hostname)}</p>");
            builder.AppendLine($"<p><strong>Process:</strong> {Encode(alert.ProcessName)}</p>");
            builder.AppendLine($"<p><strong>Command Line:</strong> <code>{Encode(alert.CommandLine)}</code></p>");
            builder.AppendLine($"<p><strong>Reason:</strong> {Encode(alert.Reason)}</p>");

            if (!string.IsNullOrWhiteSpace(alert.RuleAuthor))
            {
                builder.AppendLine($"<p><strong>Rule Author:</strong> {Encode(alert.RuleAuthor)}</p>");
            }

            if (!string.IsNullOrWhiteSpace(alert.RuleCreatedUtc))
            {
                builder.AppendLine($"<p><strong>Rule Created UTC:</strong> {Encode(alert.RuleCreatedUtc)}</p>");
            }

            if (alert.RuleTags.Count > 0)
            {
                builder.AppendLine("<p><strong>Tags:</strong></p>");

                foreach (var tag in alert.RuleTags)
                {
                    builder.AppendLine($"<span class=\"badge\">{Encode(tag)}</span>");
                }
            }

            if (alert.RuleReferences.Count > 0)
            {
                builder.AppendLine("<p><strong>References:</strong></p>");
                builder.AppendLine("<ul>");

                foreach (var reference in alert.RuleReferences)
                {
                    builder.AppendLine($"<li>{Encode(reference)}</li>");
                }

                builder.AppendLine("</ul>");
            }

            builder.AppendLine("</div>");
        }
    }

    private static int CountBySeverity(List<DetectionAlert> alerts, string severity)
    {
        return alerts.Count(alert => alert.Severity.Equals(severity, StringComparison.OrdinalIgnoreCase));
    }

    private static string Encode(string value)
    {
        return WebUtility.HtmlEncode(value ?? string.Empty);
    }
}
