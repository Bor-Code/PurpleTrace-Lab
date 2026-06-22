namespace PurpleTrace.Agent.Models;

public sealed class CliOptions
{
    public string RulesDirectory { get; set; } = "rules";
    public string OutputPath { get; set; } = "samples/latest-alerts.local.json";
}
