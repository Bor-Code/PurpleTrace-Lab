using PurpleTrace.Agent.Exporters;
using PurpleTrace.Agent.Models;

namespace PurpleTrace.Agent.Tests;

public sealed class HtmlReportExporterTests
{
    [Fact]
    public void Export_ShouldCreateHtmlReportFile()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), $"purpletrace-report-{Guid.NewGuid():N}.html");

        try
        {
            var alerts = new List<DetectionAlert>
            {
                new DetectionAlert
                {
                    RuleId = "PT-RULE-TEST",
                    RuleName = "Test Rule",
                    Severity = "High",
                    MitreTactic = "Execution",
                    MitreTechniqueId = "T1059.001",
                    MitreTechniqueName = "PowerShell",
                    RuleAuthor = "PurpleTrace Lab",
                    RuleCreatedUtc = "2026-06-22T00:00:00Z",
                    RuleTags = new List<string>
                    {
                        "powershell",
                        "test"
                    },
                    RuleReferences = new List<string>
                    {
                        "MITRE ATT&CK T1059.001 - PowerShell"
                    },
                    Hostname = "BORAN",
                    ProcessName = "powershell.exe",
                    CommandLine = "powershell.exe -NoProfile",
                    Reason = "Test alert reason."
                }
            };

            var exporter = new HtmlReportExporter();

            exporter.Export(outputPath, alerts);

            Assert.True(File.Exists(outputPath));

            var html = File.ReadAllText(outputPath);

            Assert.Contains("PurpleTrace Detection Report", html);
            Assert.Contains("PT-RULE-TEST", html);
            Assert.Contains("PurpleTrace Lab", html);
            Assert.Contains("powershell", html);
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
