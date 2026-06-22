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

        if (IsJsonLinesFile(filePath))
        {
            return LoadJsonLines(filePath);
        }

        var json = File.ReadAllText(filePath);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<EndpointEvent>();
        }

        using var document = JsonDocument.Parse(json);

        var options = CreateJsonOptions();

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

    private static bool IsJsonLinesFile(string filePath)
    {
        var extension = Path.GetExtension(filePath);

        return extension.Equals(".jsonl", StringComparison.OrdinalIgnoreCase) ||
               extension.Equals(".ndjson", StringComparison.OrdinalIgnoreCase);
    }

    private static List<EndpointEvent> LoadJsonLines(string filePath)
    {
        var events = new List<EndpointEvent>();
        var options = CreateJsonOptions();
        var lineNumber = 0;

        foreach (var line in File.ReadLines(filePath))
        {
            lineNumber++;

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            EndpointEvent? endpointEvent;

            try
            {
                endpointEvent = JsonSerializer.Deserialize<EndpointEvent>(line, options);
            }
            catch (JsonException exception)
            {
                throw new InvalidOperationException($"Invalid JSONL event at line {lineNumber} in file: {filePath}. {exception.Message}", exception);
            }

            if (endpointEvent is null)
            {
                continue;
            }

            events.Add(endpointEvent);
        }

        return events;
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }
}
