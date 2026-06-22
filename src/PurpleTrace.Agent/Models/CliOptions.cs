namespace PurpleTrace.Agent.Models;

public sealed class CliOptions
{
    public string RulesDirectory { get; set; } = "rules";
    public string EventPath { get; set; } = "samples/sample-powershell-event.json";
    public string OutputPath { get; set; } = "samples/latest-alerts.local.json";
    public string Source { get; set; } = "sample";
    public int MaxEvents { get; set; } = 20;
}
