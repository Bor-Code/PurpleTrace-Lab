using PurpleTrace.Agent.Detection;
using PurpleTrace.Agent.Models;

namespace PurpleTrace.Agent.Tests;

public sealed class AlertSeverityFilterTests
{
    [Fact]
    public void FilterByMinimumSeverity_ShouldReturnAllAlerts_WhenMinimumSeverityIsEmpty()
    {
        var alerts = new List<DetectionAlert>
        {
            new DetectionAlert { Severity = "High" },
            new DetectionAlert { Severity = "Medium" }
        };

        var filteredAlerts = AlertSeverityFilter.FilterByMinimumSeverity(alerts, string.Empty);

        Assert.Equal(2, filteredAlerts.Count);
    }

    [Fact]
    public void FilterByMinimumSeverity_ShouldReturnOnlyHighAndCritical_WhenMinimumSeverityIsHigh()
    {
        var alerts = new List<DetectionAlert>
        {
            new DetectionAlert { Severity = "Critical" },
            new DetectionAlert { Severity = "High" },
            new DetectionAlert { Severity = "Medium" },
            new DetectionAlert { Severity = "Low" }
        };

        var filteredAlerts = AlertSeverityFilter.FilterByMinimumSeverity(alerts, "High");

        Assert.Equal(2, filteredAlerts.Count);
        Assert.Contains(filteredAlerts, alert => alert.Severity == "Critical");
        Assert.Contains(filteredAlerts, alert => alert.Severity == "High");
        Assert.DoesNotContain(filteredAlerts, alert => alert.Severity == "Medium");
        Assert.DoesNotContain(filteredAlerts, alert => alert.Severity == "Low");
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenSeverityIsUnsupported()
    {
        var isValid = SeverityRanker.IsValid("VeryHigh");

        Assert.False(isValid);
    }
}
