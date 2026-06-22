using PurpleTrace.Agent.Exporters;
using PurpleTrace.Agent.Models;

namespace PurpleTrace.Agent.Tests;

public sealed class HtmlEvidenceDetailsTests
{
    [Fact]
    public void Export_ShouldIncludeEvidenceDetails_WhenAlertHasEvidence()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), $"purpletrace-evidence-report-{Guid.NewGuid():N}.html");

        try
        {
            var alerts = new List<DetectionAlert>
            {
                new DetectionAlert
                {
                    RuleId = "PT-RULE-TEST",
                    RuleName = "Test Evidence Rule",
                    Severity = "High",
                    MitreTactic = "Execution",
                    MitreTechniqueId = "T1059.001",
                    MitreTechniqueName = "PowerShell",
                    RuleAuthor = "PurpleTrace Lab",
                    RuleCreatedUtc = "2026-06-22T00:00:00Z",
                    RuleTags = new List<string>
                    {
                        "powershell",
                        "evidence"
                    },
                    RuleReferences = new List<string>
                    {
                        "MITRE ATT&CK T1059.001 - PowerShell"
                    },
                    Hostname = "BORAN",
                    UserName = "nonmr",
                    ProcessName = "powershell.exe",
                    CommandLine = "powershell.exe -NoProfile",
                    ParentProcessName = "cmd.exe",
                    Reason = "Test evidence rendering.",
                    EvidenceSummary = "ProcessName contains powershell.exe; CommandLine contains -NoProfile",
                    MatchedFields = new List<string>
                    {
                        "ProcessName",
                        "CommandLine"
                    },
                    MatchedValues = new List<string>
                    {
                        "ProcessName contains powershell.exe",
                        "CommandLine contains -NoProfile"
                    }
                }
            };

            var exporter = new HtmlReportExporter();

            exporter.Export(outputPath, alerts);

            Assert.True(File.Exists(outputPath));

            var html = File.ReadAllText(outputPath);

            Assert.Contains("Detection Evidence", html);
            Assert.Contains("Evidence Summary", html);
            Assert.Contains("Matched Fields", html);
            Assert.Contains("Matched Values", html);
            Assert.Contains("ProcessName contains powershell.exe", html);
            Assert.Contains("CommandLine contains -NoProfile", html);
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
