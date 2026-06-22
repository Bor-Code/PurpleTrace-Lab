using PurpleTrace.Agent.Exporters;
using PurpleTrace.Agent.Models;

namespace PurpleTrace.Agent.Tests;

public sealed class RunSummaryExporterTests
{
    [Fact]
    public void Export_ShouldCreateRunSummaryJsonFile()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), $"purpletrace-summary-{Guid.NewGuid():N}.json");

        try
        {
            var summary = new DetectionRunSummary
            {
                Source = "sample",
                LoadedRules = 3,
                LoadedEvents = 2,
                DetectedAlertsBeforeFiltering = 3,
                ExportedAlerts = 1,
                MinSeverity = "High",
                ActiveFilters = new List<string>
                {
                    "min-severity=High"
                },
                SeverityCounts = new Dictionary<string, int>
                {
                    ["High"] = 1
                },
                MitreTechniqueCounts = new Dictionary<string, int>
                {
                    ["T1059.001"] = 1
                },
                RuleCounts = new Dictionary<string, int>
                {
                    ["PT-RULE-001"] = 1
                }
            };

            var exporter = new RunSummaryExporter();

            exporter.Export(outputPath, summary);

            Assert.True(File.Exists(outputPath));

            var json = File.ReadAllText(outputPath);

            Assert.Contains("\"LoadedRules\": 3", json);
            Assert.Contains("\"LoadedEvents\": 2", json);
            Assert.Contains("\"DetectedAlertsBeforeFiltering\": 3", json);
            Assert.Contains("\"ExportedAlerts\": 1", json);
            Assert.Contains("min-severity=High", json);
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
