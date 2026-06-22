using PurpleTrace.Agent.Detection;

namespace PurpleTrace.Agent;

public static class RuleValidationPrinter
{
    public static void PrintSuccess(string rulesDirectory, IEnumerable<DetectionRule> rules)
    {
        var ruleList = rules.ToList();

        Console.WriteLine("PurpleTrace Rule Validation");
        Console.WriteLine();
        Console.WriteLine($"Rules directory: {rulesDirectory}");
        Console.WriteLine("Validation passed.");
        Console.WriteLine($"Valid rules: {ruleList.Count}");
    }

    public static void PrintFailure(string rulesDirectory, Exception exception)
    {
        Console.WriteLine("PurpleTrace Rule Validation");
        Console.WriteLine();
        Console.WriteLine($"Rules directory: {rulesDirectory}");
        Console.WriteLine("Validation failed.");
        Console.WriteLine();
        Console.WriteLine(exception.Message);
    }
}
