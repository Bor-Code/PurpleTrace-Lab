using PurpleTrace.Agent.Models;

namespace PurpleTrace.Agent.Tests;

public sealed class CliOptionsParserTests
{
    [Fact]
    public void ApplyCommandLineOverrides_ShouldOverrideExistingValues()
    {
        var options = new CliOptions
        {
            Source = "sysmon",
            MinSeverity = string.Empty,
            MaxEvents = 20
        };

        CliOptionsParser.ApplyCommandLineOverrides(options, new[]
        {
            "--source", "sample",
            "--min-severity", "High",
            "--max", "5"
        });

        Assert.Equal("sample", options.Source);
        Assert.Equal("High", options.MinSeverity);
        Assert.Equal(5, options.MaxEvents);
    }

    [Fact]
    public void ApplyCommandLineOverrides_ShouldSetConfigPath()
    {
        var options = new CliOptions();

        CliOptionsParser.ApplyCommandLineOverrides(options, new[]
        {
            "--config", "config/purpletrace.sample.json"
        });

        Assert.Equal("config/purpletrace.sample.json", options.ConfigPath);
    }

    [Fact]
    public void ApplyCommandLineOverrides_ShouldSetListRulesFlag()
    {
        var options = new CliOptions();

        CliOptionsParser.ApplyCommandLineOverrides(options, new[]
        {
            "--list-rules"
        });

        Assert.True(options.ListRules);
    }
}
