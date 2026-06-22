using PurpleTrace.Agent.Models;

namespace PurpleTrace.Agent.Detection;

public static class AlertMitreTechniqueFilter
{
    public static List<DetectionAlert> FilterByTechniqueId(IEnumerable<DetectionAlert> alerts, string mitreTechniqueId)
    {
        var alertList = alerts.ToList();

        if (string.IsNullOrWhiteSpace(mitreTechniqueId))
        {
            return alertList;
        }

        return alertList
            .Where(alert => alert.MitreTechniqueId.Equals(mitreTechniqueId, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}
