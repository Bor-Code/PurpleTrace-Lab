namespace PurpleTrace.Agent.Models;

public sealed class DetectionAlert
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

    public string RuleId { get; set; } = string.Empty;
    public string RuleName { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;

    public string MitreTactic { get; set; } = string.Empty;
    public string MitreTechniqueId { get; set; } = string.Empty;
    public string MitreTechniqueName { get; set; } = string.Empty;

    public string Hostname { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public string CommandLine { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;

    public EndpointEvent? SourceEvent { get; set; }
}
