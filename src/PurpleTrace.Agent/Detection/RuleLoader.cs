using System.Text.Json;

namespace PurpleTrace.Agent.Detection;

public sealed class RuleLoader
{
    public List<DetectionRule> LoadFromDirectory(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException($"Rule directory not found: {directoryPath}");
        }

        var rules = new List<DetectionRule>();
        var files = Directory.GetFiles(directoryPath, "*.json", SearchOption.TopDirectoryOnly);

        foreach (var file in files)
        {
            var json = File.ReadAllText(file);

            var rule = JsonSerializer.Deserialize<DetectionRule>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (rule is not null && !string.IsNullOrWhiteSpace(rule.Id))
            {
                rules.Add(rule);
            }
        }

        return rules;
    }
}
