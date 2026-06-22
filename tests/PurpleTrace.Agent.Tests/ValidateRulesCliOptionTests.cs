using PurpleTrace.Agent.Models;

namespace PurpleTrace.Agent.Tests;

public sealed class ValidateRulesCliOptionTests
{
    [Fact]
    public void ApplyCommandLineOverrides_ShouldSetValidateRulesFlag()
    {
        var options = new CliOptions();

        CliOptionsParser.ApplyCommandLineOverrides(options, new[]
        {
            "--validate-rules"
        });

        Assert.True(options.ValidateRules);
    }

    [Fact]
    public void ApplyCommandLineOverrides_ShouldKeepValidateRulesFalse_WhenOptionIsMissing()
    {
        var options = new CliOptions();

        CliOptionsParser.ApplyCommandLineOverrides(options, Array.Empty<string>());

        Assert.False(options.ValidateRules);
    }
}
