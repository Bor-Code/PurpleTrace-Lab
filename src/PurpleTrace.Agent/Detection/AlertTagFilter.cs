using PurpleTrace.Agent.Models;

namespace PurpleTrace.Agent.Detection;

public static class AlertTagFilter
{
    public static List<DetectionAlert> FilterByTag(IEnumerable<DetectionAlert> alerts, string tag)
    {
        var alertList = alerts.ToList();

        if (string.IsNullOrWhiteSpace(tag))
        {
            return alertList;
        }

        return alertList
            .Where(alert => alert.RuleTags.Any(ruleTag => ruleTag.Equals(tag, StringComparison.OrdinalIgnoreCase)))
            .ToList();
    }
}
