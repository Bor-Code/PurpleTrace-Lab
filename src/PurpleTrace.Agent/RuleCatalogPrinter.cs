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
            Console.WriteLine();
        }
    }
}
