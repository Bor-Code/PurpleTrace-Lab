using PurpleTrace.Agent.Detection;
using PurpleTrace.Agent.Exporters;

namespace PurpleTrace.Agent.Tests;

public sealed class RuleCoverageMarkdownExporterTests
{
    [Fact]
    public void Export_ShouldCreateRuleCoverageMarkdownFile()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), $"purpletrace-rule-coverage-{Guid.NewGuid():N}.md");

        try
        {
            var rules = new List<DetectionRule>
            {
                new DetectionRule
                {
                    Id = "PT-RULE-001",
                    Title = "Suspicious PowerShell Execution",
                    Description = "Detects suspicious PowerShell command-line arguments.",
                    Severity = "High",
                    MitreTactic = "Execution",
                    MitreTechniqueId = "T1059.001",
                    MitreTechniqueName = "PowerShell",
                    Author = "PurpleTrace Lab",
                    CreatedUtc = "2026-06-22T00:00:00Z",
                    Tags = new List<string>
                    {
                        "powershell",
                        "execution"
                    },
                    References = new List<string>
                    {
                        "MITRE ATT&CK T1059.001 - PowerShell"
                    },
                    ProcessNameContains = new List<string>
                    {
                        "powershell.exe"
                    },
                    CommandLineContains = new List<string>
                    {
                        "-NoProfile"
                    },
                    ParentProcessNameContains = new List<string>()
                }
            };

            var exporter = new RuleCoverageMarkdownExporter();

            exporter.Export(outputPath, rules);

            Assert.True(File.Exists(outputPath));

            var markdown = File.ReadAllText(outputPath);

            Assert.Contains("# PurpleTrace Rule Coverage", markdown);
            Assert.Contains("Total Rules: 1", markdown);
            Assert.Contains("## Severity Coverage", markdown);
            Assert.Contains("## MITRE Coverage", markdown);
            Assert.Contains("## Tag Coverage", markdown);
            Assert.Contains("## Rule Table", markdown);
            Assert.Contains("PT-RULE-001", markdown);
            Assert.Contains("T1059.001", markdown);
            Assert.Contains("powershell", markdown);
        }
        finally
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
        }
    }
}
