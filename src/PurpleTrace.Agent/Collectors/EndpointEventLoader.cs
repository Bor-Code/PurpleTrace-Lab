using System.Text.Json;
using PurpleTrace.Agent.Models;

namespace PurpleTrace.Agent.Collectors;

public sealed class EndpointEventLoader
{
    public EndpointEvent LoadFromJsonFile(string filePath)
    {
        var events = LoadManyFromJsonFile(filePath);

        if (events.Count == 0)
        {
            throw new InvalidOperationException($"No endpoint event found in: {filePath}");
        }

        return events[0];
    }

    public List<EndpointEvent> LoadManyFromJsonFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Event file not found: {filePath}");
        }

        var json = File.ReadAllText(filePath);

        using var document = JsonDocument.Parse(json);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        if (document.RootElement.ValueKind == JsonValueKind.Array)
        {
            var events = JsonSerializer.Deserialize<List<EndpointEvent>>(json, options);

            return events ?? new List<EndpointEvent>();
        }

        if (document.RootElement.ValueKind == JsonValueKind.Object)
        {
            var endpointEvent = JsonSerializer.Deserialize<EndpointEvent>(json, options);

            if (endpointEvent is null)
            {
                return new List<EndpointEvent>();
            }

            return new List<EndpointEvent>
            {
                endpointEvent
            };
        }

        throw new InvalidOperationException($"Unsupported event JSON format: {filePath}");
    }
}
