using PurpleTrace.Agent.Detection;

namespace PurpleTrace.Agent;

public static class RuleCatalogPrinter
{
    public static void Print(IEnumerable<DetectionRule> rules)
    {
        var ruleList = rules
            .OrderBy(rule => rule.Id)
            .ToList();

        Console.WriteLine("PurpleTrace Detection Rule Catalog");
        Console.WriteLine();
        Console.WriteLine($"Total Rules: {ruleList.Count}");
        Console.WriteLine();

        if (ruleList.Count == 0)
        {
            Console.WriteLine("No rules found.");
            return;
        }

        foreach (var rule in ruleList)
        {
            Console.WriteLine($"{rule.Id} | {rule.Severity} | {rule.MitreTechniqueId} | {rule.Title}");
            Console.WriteLine($"  Tactic: {rule.MitreTactic}");
            Console.WriteLine($"  Technique: {rule.MitreTechniqueName}");
            Console.WriteLine($"  Description: {rule.Description}");

            if (!string.IsNullOrWhiteSpace(rule.Author))
            {
                Console.WriteLine($"  Author: {rule.Author}");
            }

            if (!string.IsNullOrWhiteSpace(rule.CreatedUtc))
            {
                Console.WriteLine($"  Created UTC: {rule.CreatedUtc}");
            }

            if (rule.Tags.Count > 0)
            {
                Console.WriteLine($"  Tags: {string.Join(", ", rule.Tags)}");
            }

            if (rule.References.Count > 0)
            {
                Console.WriteLine("  References:");

                foreach (var reference in rule.References)
                {
                    Console.WriteLine($"    - {reference}");
                }
            }

            Console.WriteLine();
        }
    }
}
