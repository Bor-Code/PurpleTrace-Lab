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

            if (rule is null)
            {
                throw new InvalidOperationException($"Could not deserialize rule file: {file}");
            }

            var validationResult = RuleValidator.Validate(rule);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(Environment.NewLine, validationResult.Errors.Select(error => $"- {error}"));
                throw new InvalidOperationException($"Invalid rule file: {file}{Environment.NewLine}{errors}");
            }

            rules.Add(rule);
        }

        return rules;
    }
}
