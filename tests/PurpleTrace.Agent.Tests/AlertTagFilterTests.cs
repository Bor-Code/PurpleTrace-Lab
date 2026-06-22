using PurpleTrace.Agent.Detection;
using PurpleTrace.Agent.Models;

namespace PurpleTrace.Agent.Tests;

public sealed class AlertTagFilterTests
{
    [Fact]
    public void FilterByTag_ShouldReturnAllAlerts_WhenTagIsEmpty()
    {
        var alerts = new List<DetectionAlert>
        {
            new DetectionAlert { RuleTags = new List<string> { "powershell" } },
            new DetectionAlert { RuleTags = new List<string> { "discovery" } }
        };

        var filteredAlerts = AlertTagFilter.FilterByTag(alerts, string.Empty);

        Assert.Equal(2, filteredAlerts.Count);
    }

    [Fact]
    public void FilterByTag_ShouldReturnOnlyMatchingTag()
    {
        var alerts = new List<DetectionAlert>
        {
            new DetectionAlert { RuleId = "PT-RULE-001", RuleTags = new List<string> { "powershell", "execution" } },
            new DetectionAlert { RuleId = "PT-RULE-003", RuleTags = new List<string> { "discovery", "windows" } }
        };

        var filteredAlerts = AlertTagFilter.FilterByTag(alerts, "discovery");

        Assert.Single(filteredAlerts);
        Assert.Equal("PT-RULE-003", filteredAlerts[0].RuleId);
    }

    [Fact]
    public void FilterByTag_ShouldBeCaseInsensitive()
    {
        var alerts = new List<DetectionAlert>
        {
            new DetectionAlert { RuleTags = new List<string> { "Discovery" } }
        };

        var filteredAlerts = AlertTagFilter.FilterByTag(alerts, "discovery");

        Assert.Single(filteredAlerts);
    }
}
