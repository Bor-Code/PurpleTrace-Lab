using PurpleTrace.Agent.Detection;

namespace PurpleTrace.Agent.Tests;

public sealed class RuleValidatorTests
{
    [Fact]
    public void Validate_ShouldReturnValid_WhenRuleHasRequiredFields()
    {
        var rule = new DetectionRule
        {
            Id = "PT-RULE-TEST",
            Title = "Test Rule",
            Description = "Test description.",
            Severity = "Medium",
            MitreTactic = "Discovery",
            MitreTechniqueId = "T1082",
            MitreTechniqueName = "System Information Discovery",
            ProcessNameContains = new List<string>
            {
                "cmd.exe"
            }
        };

        var result = RuleValidator.Validate(rule);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_ShouldReturnInvalid_WhenRequiredFieldsAreMissing()
    {
        var rule = new DetectionRule();

        var result = RuleValidator.Validate(rule);

        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Validate_ShouldReturnInvalid_WhenSeverityIsUnsupported()
    {
        var rule = new DetectionRule
        {
            Id = "PT-RULE-TEST",
            Title = "Test Rule",
            Description = "Test description.",
            Severity = "VeryHigh",
            MitreTactic = "Discovery",
            MitreTechniqueId = "T1082",
            MitreTechniqueName = "System Information Discovery",
            ProcessNameContains = new List<string>
            {
                "cmd.exe"
            }
        };

        var result = RuleValidator.Validate(rule);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.Contains("Invalid Severity value"));
    }
}
