using System.Text.Json;
using PurpleTrace.Agent.Models;

namespace PurpleTrace.Agent.Collectors;

public sealed class EndpointEventLoader
{
    public EndpointEvent LoadFromJsonFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Event file not found: {filePath}");
        }

        var json = File.ReadAllText(filePath);

        var endpointEvent = JsonSerializer.Deserialize<EndpointEvent>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (endpointEvent is null)
        {
            throw new InvalidOperationException($"Could not deserialize endpoint event from: {filePath}");
        }

        return endpointEvent;
    }
}
