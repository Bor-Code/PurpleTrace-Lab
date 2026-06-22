using PurpleTrace.Agent.Models;

namespace PurpleTrace.Agent.Detection;

public static class AlertRuleIdFilter
{
    public static List<DetectionAlert> FilterByRuleId(IEnumerable<DetectionAlert> alerts, string ruleId)
    {
        var alertList = alerts.ToList();

        if (string.IsNullOrWhiteSpace(ruleId))
        {
            return alertList;
        }

        return alertList
            .Where(alert => alert.RuleId.Equals(ruleId, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}
