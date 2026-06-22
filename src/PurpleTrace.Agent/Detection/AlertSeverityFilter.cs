using PurpleTrace.Agent.Models;

namespace PurpleTrace.Agent.Detection;

public static class AlertSeverityFilter
{
    public static List<DetectionAlert> FilterByMinimumSeverity(IEnumerable<DetectionAlert> alerts, string minimumSeverity)
    {
        var alertList = alerts.ToList();

        if (string.IsNullOrWhiteSpace(minimumSeverity))
        {
            return alertList;
        }

        var minimumRank = SeverityRanker.GetRank(minimumSeverity);

        return alertList
            .Where(alert => SeverityRanker.GetRank(alert.Severity) >= minimumRank)
            .ToList();
    }
}
