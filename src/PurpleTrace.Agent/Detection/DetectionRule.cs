namespace PurpleTrace.Agent.Detection;

public sealed class DetectionRule
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = "Low";

    public string MitreTactic { get; set; } = string.Empty;
    public string MitreTechniqueId { get; set; } = string.Empty;
    public string MitreTechniqueName { get; set; } = string.Empty;

    public List<string> ProcessNameContains { get; set; } = new();
    public List<string> CommandLineContains { get; set; } = new();
    public List<string> ParentProcessNameContains { get; set; } = new();
}
