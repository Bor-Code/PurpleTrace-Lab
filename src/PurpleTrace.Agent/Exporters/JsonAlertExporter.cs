using System.Text.Json;
using PurpleTrace.Agent.Models;

namespace PurpleTrace.Agent.Exporters;

public sealed class JsonAlertExporter
{
    public void Export(string outputPath, IEnumerable<DetectionAlert> alerts)
    {
        var directory = Path.GetDirectoryName(outputPath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(alerts, options);
        File.WriteAllText(outputPath, json);
    }
}
