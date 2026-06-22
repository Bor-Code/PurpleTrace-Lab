using PurpleTrace.Agent.Detection;
using PurpleTrace.Agent.Models;

namespace PurpleTrace.Agent.Tests;

public sealed class AlertRuleIdFilterTests
{
    [Fact]
    public void FilterByRuleId_ShouldReturnAllAlerts_WhenRuleIdIsEmpty()
    {
        var alerts = new List<DetectionAlert>
        {
            new DetectionAlert { RuleId = "PT-RULE-001" },
            new DetectionAlert { RuleId = "PT-RULE-003" }
        };

        var filteredAlerts = AlertRuleIdFilter.FilterByRuleId(alerts, string.Empty);

        Assert.Equal(2, filteredAlerts.Count);
    }

    [Fact]
    public void FilterByRuleId_ShouldReturnOnlyMatchingRule()
    {
        var alerts = new List<DetectionAlert>
        {
            new DetectionAlert { RuleId = "PT-RULE-001" },
            new DetectionAlert { RuleId = "PT-RULE-002" },
            new DetectionAlert { RuleId = "PT-RULE-003" }
        };

        var filteredAlerts = AlertRuleIdFilter.FilterByRuleId(alerts, "PT-RULE-003");

        Assert.Single(filteredAlerts);
        Assert.Equal("PT-RULE-003", filteredAlerts[0].RuleId);
    }

    [Fact]
    public void FilterByRuleId_ShouldBeCaseInsensitive()
    {
        var alerts = new List<DetectionAlert>
        {
            new DetectionAlert { RuleId = "PT-RULE-003" }
        };

        var filteredAlerts = AlertRuleIdFilter.FilterByRuleId(alerts, "pt-rule-003");

        Assert.Single(filteredAlerts);
    }
}
