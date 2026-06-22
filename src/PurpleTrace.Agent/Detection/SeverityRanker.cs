namespace PurpleTrace.Agent.Detection;

public static class SeverityRanker
{
    private static readonly Dictionary<string, int> Rankings = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Informational"] = 1,
        ["Low"] = 2,
        ["Medium"] = 3,
        ["High"] = 4,
        ["Critical"] = 5
    };

    public static string ValidValues => "Critical, High, Medium, Low, Informational";

    public static bool IsValid(string severity)
    {
        return Rankings.ContainsKey(severity);
    }

    public static int GetRank(string severity)
    {
        return Rankings.TryGetValue(severity, out var rank) ? rank : 0;
    }
}
