using PurpleTrace.Agent.Detection;
using PurpleTrace.Agent.Models;

namespace PurpleTrace.Agent.Tests;

public sealed class AlertMitreTechniqueFilterTests
{
    [Fact]
    public void FilterByTechniqueId_ShouldReturnAllAlerts_WhenTechniqueIdIsEmpty()
    {
        var alerts = new List<DetectionAlert>
        {
            new DetectionAlert { MitreTechniqueId = "T1059.001" },
            new DetectionAlert { MitreTechniqueId = "T1082" }
        };

        var filteredAlerts = AlertMitreTechniqueFilter.FilterByTechniqueId(alerts, string.Empty);

        Assert.Equal(2, filteredAlerts.Count);
    }

    [Fact]
    public void FilterByTechniqueId_ShouldReturnOnlyMatchingTechnique()
    {
        var alerts = new List<DetectionAlert>
        {
            new DetectionAlert { MitreTechniqueId = "T1059.001" },
            new DetectionAlert { MitreTechniqueId = "T1059" },
            new DetectionAlert { MitreTechniqueId = "T1082" }
        };

        var filteredAlerts = AlertMitreTechniqueFilter.FilterByTechniqueId(alerts, "T1082");

        Assert.Single(filteredAlerts);
        Assert.Equal("T1082", filteredAlerts[0].MitreTechniqueId);
    }

    [Fact]
    public void FilterByTechniqueId_ShouldBeCaseInsensitive()
    {
        var alerts = new List<DetectionAlert>
        {
            new DetectionAlert { MitreTechniqueId = "T1082" }
        };

        var filteredAlerts = AlertMitreTechniqueFilter.FilterByTechniqueId(alerts, "t1082");

        Assert.Single(filteredAlerts);
    }
}
