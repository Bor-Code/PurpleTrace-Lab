using System.Text.Json;

namespace PurpleTrace.Agent.Config;

public sealed class AgentConfigLoader
{
    public AgentConfig Load(string configPath)
    {
        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException($"Config file not found: {configPath}");
        }

        var json = File.ReadAllText(configPath);

        var config = JsonSerializer.Deserialize<AgentConfig>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (config is null)
        {
            throw new InvalidOperationException($"Could not deserialize config file: {configPath}");
        }

        return config;
    }
}
