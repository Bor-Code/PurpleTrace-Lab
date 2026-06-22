namespace PurpleTrace.Agent.Models;

public sealed class EndpointEvent
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    public string Source { get; set; } = string.Empty;
    public int EventId { get; set; }
    public EndpointEventType EventType { get; set; } = EndpointEventType.Unknown;

    public string Hostname { get; set; } = Environment.MachineName;
    public string UserName { get; set; } = string.Empty;

    public string ProcessGuid { get; set; } = string.Empty;
    public int ProcessId { get; set; }
    public string ProcessName { get; set; } = string.Empty;
    public string ProcessPath { get; set; } = string.Empty;
    public string CommandLine { get; set; } = string.Empty;

    public string ParentProcessGuid { get; set; } = string.Empty;
    public int ParentProcessId { get; set; }
    public string ParentProcessName { get; set; } = string.Empty;
    public string ParentProcessPath { get; set; } = string.Empty;
    public string ParentCommandLine { get; set; } = string.Empty;

    public string DestinationIp { get; set; } = string.Empty;
    public int DestinationPort { get; set; }
    public string Protocol { get; set; } = string.Empty;

    public Dictionary<string, string> AdditionalData { get; set; } = new();
}
