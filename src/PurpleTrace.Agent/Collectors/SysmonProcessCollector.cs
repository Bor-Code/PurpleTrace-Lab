using System.Diagnostics.Eventing.Reader;
using System.Xml.Linq;
using PurpleTrace.Agent.Models;

namespace PurpleTrace.Agent.Collectors;

public sealed class SysmonProcessCollector
{
    private const string SysmonChannel = "Microsoft-Windows-Sysmon/Operational";

    public List<EndpointEvent> GetRecentProcessCreateEvents(int maxEvents)
    {
        var events = new List<EndpointEvent>();

        var query = new EventLogQuery(
            SysmonChannel,
            PathType.LogName,
            "*[System[(EventID=1)]]"
        )
        {
            ReverseDirection = true
        };

        using var reader = new EventLogReader(query);

        while (events.Count < maxEvents)
        {
            using var record = reader.ReadEvent();

            if (record is null)
            {
                break;
            }

            var endpointEvent = ConvertToEndpointEvent(record);

            if (endpointEvent is not null)
            {
                events.Add(endpointEvent);
            }
        }

        return events;
    }

    private static EndpointEvent? ConvertToEndpointEvent(EventRecord record)
    {
        var xml = record.ToXml();

        if (string.IsNullOrWhiteSpace(xml))
        {
            return null;
        }

        var document = XDocument.Parse(xml);

        var eventData = document
            .Descendants()
            .Where(element => element.Name.LocalName == "Data")
            .ToDictionary(
                element => element.Attribute("Name")?.Value ?? string.Empty,
                element => element.Value
            );

        var image = GetValue(eventData, "Image");
        var parentImage = GetValue(eventData, "ParentImage");

        return new EndpointEvent
        {
            Source = "Sysmon",
            EventId = record.Id,
            EventType = EndpointEventType.ProcessCreate,
            TimestampUtc = record.TimeCreated?.ToUniversalTime() ?? DateTime.UtcNow,
            Hostname = Environment.MachineName,

            UserName = GetValue(eventData, "User"),

            ProcessGuid = GetValue(eventData, "ProcessGuid"),
            ProcessId = TryParseInt(GetValue(eventData, "ProcessId")),
            ProcessName = Path.GetFileName(image),
            ProcessPath = image,
            CommandLine = GetValue(eventData, "CommandLine"),

            ParentProcessGuid = GetValue(eventData, "ParentProcessGuid"),
            ParentProcessId = TryParseInt(GetValue(eventData, "ParentProcessId")),
            ParentProcessName = Path.GetFileName(parentImage),
            ParentProcessPath = parentImage,
            ParentCommandLine = GetValue(eventData, "ParentCommandLine"),

            AdditionalData = eventData
        };
    }

    private static string GetValue(Dictionary<string, string> data, string key)
    {
        return data.TryGetValue(key, out var value) ? value : string.Empty;
    }

    private static int TryParseInt(string value)
    {
        return int.TryParse(value, out var result) ? result : 0;
    }
}
