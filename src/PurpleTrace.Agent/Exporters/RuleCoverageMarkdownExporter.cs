using System.Text;
using PurpleTrace.Agent.Detection;

namespace PurpleTrace.Agent.Exporters;

public sealed class RuleCoverageMarkdownExporter
{
    public void Export(string outputPath, IEnumerable<DetectionRule> rules)
    {
        var ruleList = rules
            .OrderBy(rule => rule.Id)
            .ToList();

        var directory = Path.GetDirectoryName(outputPath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var builder = new StringBuilder();

        builder.AppendLine("# PurpleTrace Rule Coverage");
        builder.AppendLine();
        builder.AppendLine($"Generated UTC: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
        builder.AppendLine();
        builder.AppendLine($"Total Rules: {ruleList.Count}");
        builder.AppendLine();

        AppendSeverityCoverage(builder, ruleList);
        AppendMitreCoverage(builder, ruleList);
        AppendTagCoverage(builder, ruleList);
        AppendRuleTable(builder, ruleList);
        AppendRuleDetails(builder, ruleList);

        File.WriteAllText(outputPath, builder.ToString());
    }

    private static void AppendSeverityCoverage(StringBuilder builder, List<DetectionRule> rules)
    {
        builder.AppendLine("## Severity Coverage");
        builder.AppendLine();
        builder.AppendLine("| Severity | Count |");
        builder.AppendLine("|---|---|");

        foreach (var group in rules.GroupBy(rule => rule.Severity).OrderBy(group => group.Key))
        {
            builder.AppendLine($"| {Escape(group.Key)} | {group.Count()} |");
        }

        builder.AppendLine();
    }

    private static void AppendMitreCoverage(StringBuilder builder, List<DetectionRule> rules)
    {
        builder.AppendLine("## MITRE Coverage");
        builder.AppendLine();
        builder.AppendLine("| Technique ID | Technique Name | Rule Count |");
        builder.AppendLine("|---|---|---|");

        foreach (var group in rules.GroupBy(rule => new { rule.MitreTechniqueId, rule.MitreTechniqueName }).OrderBy(group => group.Key.MitreTechniqueId))
        {
            builder.AppendLine($"| {Escape(group.Key.MitreTechniqueId)} | {Escape(group.Key.MitreTechniqueName)} | {group.Count()} |");
        }

        builder.AppendLine();
    }

    private static void AppendTagCoverage(StringBuilder builder, List<DetectionRule> rules)
    {
        builder.AppendLine("## Tag Coverage");
        builder.AppendLine();
        builder.AppendLine("| Tag | Rule Count |");
        builder.AppendLine("|---|---|");

        var tagGroups = rules
            .SelectMany(rule => rule.Tags)
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .GroupBy(tag => tag, StringComparer.OrdinalIgnoreCase)
            .OrderBy(group => group.Key);

        foreach (var group in tagGroups)
        {
            builder.AppendLine($"| {Escape(group.Key)} | {group.Count()} |");
        }

        builder.AppendLine();
    }

    private static void AppendRuleTable(StringBuilder builder, List<DetectionRule> rules)
    {
        builder.AppendLine("## Rule Table");
        builder.AppendLine();
        builder.AppendLine("| Rule ID | Title | Severity | MITRE | Tags |");
        builder.AppendLine("|---|---|---|---|---|");

        foreach (var rule in rules)
        {
            var mitre = $"{rule.MitreTechniqueId} {rule.MitreTechniqueName}".Trim();
            var tags = string.Join(", ", rule.Tags);

            builder.AppendLine($"| {Escape(rule.Id)} | {Escape(rule.Title)} | {Escape(rule.Severity)} | {Escape(mitre)} | {Escape(tags)} |");
        }

        builder.AppendLine();
    }

    private static void AppendRuleDetails(StringBuilder builder, List<DetectionRule> rules)
    {
        builder.AppendLine("## Rule Details");
        builder.AppendLine();

        foreach (var rule in rules)
        {
            builder.AppendLine($"### {Escape(rule.Id)} - {Escape(rule.Title)}");
            builder.AppendLine();
            builder.AppendLine($"- Severity: {Escape(rule.Severity)}");
            builder.AppendLine($"- MITRE Tactic: {Escape(rule.MitreTactic)}");
            builder.AppendLine($"- MITRE Technique: {Escape(rule.MitreTechniqueId)} - {Escape(rule.MitreTechniqueName)}");

            if (!string.IsNullOrWhiteSpace(rule.Author))
            {
                builder.AppendLine($"- Author: {Escape(rule.Author)}");
            }

            if (!string.IsNullOrWhiteSpace(rule.CreatedUtc))
            {
                builder.AppendLine($"- Created UTC: {Escape(rule.CreatedUtc)}");
            }

            if (rule.Tags.Count > 0)
            {
                builder.AppendLine($"- Tags: {Escape(string.Join(", ", rule.Tags))}");
            }

            builder.AppendLine($"- Description: {Escape(rule.Description)}");
            builder.AppendLine();

            AppendConditionList(builder, "ProcessNameContains", rule.ProcessNameContains);
            AppendConditionList(builder, "CommandLineContains", rule.CommandLineContains);
            AppendConditionList(builder, "ParentProcessNameContains", rule.ParentProcessNameContains);

            if (rule.References.Count > 0)
            {
                builder.AppendLine("References:");

                foreach (var reference in rule.References)
                {
                    builder.AppendLine($"- {Escape(reference)}");
                }

                builder.AppendLine();
            }
        }
    }

    private static void AppendConditionList(StringBuilder builder, string title, List<string> values)
    {
        if (values.Count == 0)
        {
            return;
        }

        builder.AppendLine($"{title}:");

        foreach (var value in values)
        {
            builder.AppendLine($"- `{Escape(value)}`");
        }

        builder.AppendLine();
    }

    private static string Escape(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.Replace("|", "\\|");
    }
}
