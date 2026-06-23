using System.Text.Json;

var options = SimulatorOptions.Parse(args);

if (options.ShowHelp)
{
    PrintHelp();
    return;
}

var events = SyntheticEventFactory.Create(options.Scenario);

if (events.Count == 0)
{
    Console.WriteLine($"No events generated for scenario: {options.Scenario}");
    Environment.ExitCode = 1;
    return;
}

var outputPath = ResolvePath(options.OutputPath);
var outputDirectory = Path.GetDirectoryName(outputPath);

if (!string.IsNullOrWhiteSpace(outputDirectory))
{
    Directory.CreateDirectory(outputDirectory);
}

if (options.Format.Equals("jsonl", StringComparison.OrdinalIgnoreCase) ||
    options.Format.Equals("ndjson", StringComparison.OrdinalIgnoreCase))
{
    WriteJsonLines(outputPath, events);
}
else if (options.Format.Equals("json", StringComparison.OrdinalIgnoreCase))
{
    WriteJsonArray(outputPath, events);
}
else
{
    Console.WriteLine($"Invalid format: {options.Format}");
    Console.WriteLine("Supported formats: json, jsonl, ndjson");
    Environment.ExitCode = 1;
    return;
}

Console.WriteLine("PurpleTrace Simulator");
Console.WriteLine();
Console.WriteLine($"Scenario: {options.Scenario}");
Console.WriteLine($"Format: {options.Format}");
Console.WriteLine($"Generated events: {events.Count}");
Console.WriteLine($"Output path: {outputPath}");

static void WriteJsonArray(string outputPath, List<SyntheticEndpointEvent> events)
{
    var jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true
    };

    File.WriteAllText(outputPath, JsonSerializer.Serialize(events, jsonOptions));
}

static void WriteJsonLines(string outputPath, List<SyntheticEndpointEvent> events)
{
    var lines = events.Select(endpointEvent => JsonSerializer.Serialize(endpointEvent));
    File.WriteAllLines(outputPath, lines);
}

static string ResolvePath(string path)
{
    if (Path.IsPathRooted(path))
    {
        return path;
    }

    return Path.Combine(Directory.GetCurrentDirectory(), path);
}

static void PrintHelp()
{
    Console.WriteLine("PurpleTrace Simulator");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  PurpleTrace.Simulator --scenario all --format jsonl --out samples/simulated-events.local.jsonl");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  --scenario    Scenario name. Values: all, powershell, encoded-powershell, certutil, registry-discovery, rundll32-url, service-discovery");
    Console.WriteLine("  --format      Output format. Values: json, jsonl, ndjson");
    Console.WriteLine("  --out         Output file path");
    Console.WriteLine("  --help        Show help");
    Console.WriteLine();
    Console.WriteLine("Examples:");
    Console.WriteLine("  dotnet run --project src\\PurpleTrace.Simulator -- --scenario all --format jsonl --out samples\\simulated-events.local.jsonl");
    Console.WriteLine("  dotnet run --project src\\PurpleTrace.Simulator -- --scenario encoded-powershell --format json --out samples\\encoded-simulated.local.json");
}

public sealed class SimulatorOptions
{
    public string Scenario { get; set; } = "all";
    public string Format { get; set; } = "jsonl";
    public string OutputPath { get; set; } = "samples/simulated-events.local.jsonl";
    public bool ShowHelp { get; set; }

    public static SimulatorOptions Parse(string[] args)
    {
        var options = new SimulatorOptions();

        for (var i = 0; i < args.Length; i++)
        {
            var current = args[i];

            if (current.Equals("--help", StringComparison.OrdinalIgnoreCase) ||
                current.Equals("-h", StringComparison.OrdinalIgnoreCase) ||
                current.Equals("/?", StringComparison.OrdinalIgnoreCase))
            {
                options.ShowHelp = true;
                continue;
            }

            if (current.Equals("--scenario", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                options.Scenario = args[i + 1];
                i++;
                continue;
            }

            if (current.Equals("--format", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                options.Format = args[i + 1];
                i++;
                continue;
            }

            if (current.Equals("--out", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                options.OutputPath = args[i + 1];
                i++;
                continue;
            }
        }

        return options;
    }
}

public static class SyntheticEventFactory
{
    public static List<SyntheticEndpointEvent> Create(string scenario)
    {
        if (scenario.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            return new List<SyntheticEndpointEvent>
            {
                CreatePowerShell(),
                CreateEncodedPowerShell(),
                CreateCertutil(),
                CreateRegistryDiscovery(),
                CreateRundll32Url(),
                CreateServiceDiscovery()
            };
        }

        if (scenario.Equals("powershell", StringComparison.OrdinalIgnoreCase))
        {
            return new List<SyntheticEndpointEvent> { CreatePowerShell() };
        }

        if (scenario.Equals("encoded-powershell", StringComparison.OrdinalIgnoreCase))
        {
            return new List<SyntheticEndpointEvent> { CreateEncodedPowerShell() };
        }

        if (scenario.Equals("certutil", StringComparison.OrdinalIgnoreCase))
        {
            return new List<SyntheticEndpointEvent> { CreateCertutil() };
        }

        if (scenario.Equals("registry-discovery", StringComparison.OrdinalIgnoreCase))
        {
            return new List<SyntheticEndpointEvent> { CreateRegistryDiscovery() };
        }

        if (scenario.Equals("rundll32-url", StringComparison.OrdinalIgnoreCase))
        {
            return new List<SyntheticEndpointEvent> { CreateRundll32Url() };
        }

        if (scenario.Equals("service-discovery", StringComparison.OrdinalIgnoreCase))
        {
            return new List<SyntheticEndpointEvent> { CreateServiceDiscovery() };
        }

        return new List<SyntheticEndpointEvent>();
    }

    private static SyntheticEndpointEvent CreatePowerShell()
    {
        return CreateProcessEvent(
            processId: 4100,
            processName: "powershell.exe",
            processPath: "C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe",
            commandLine: "powershell.exe -NoProfile -ExecutionPolicy Bypass",
            parentProcessId: 3000,
            parentProcessName: "cmd.exe",
            parentProcessPath: "C:\\Windows\\System32\\cmd.exe",
            parentCommandLine: "cmd.exe");
    }

    private static SyntheticEndpointEvent CreateEncodedPowerShell()
    {
        return CreateProcessEvent(
            processId: 4200,
            processName: "powershell.exe",
            processPath: "C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe",
            commandLine: "powershell.exe -NoProfile -EncodedCommand SAFE_SAMPLE_ONLY",
            parentProcessId: 3000,
            parentProcessName: "cmd.exe",
            parentProcessPath: "C:\\Windows\\System32\\cmd.exe",
            parentCommandLine: "cmd.exe");
    }

    private static SyntheticEndpointEvent CreateCertutil()
    {
        return CreateProcessEvent(
            processId: 4300,
            processName: "certutil.exe",
            processPath: "C:\\Windows\\System32\\certutil.exe",
            commandLine: "certutil.exe -urlcache -f https://example.com/SAFE_SAMPLE_ONLY.txt SAFE_SAMPLE_ONLY.txt",
            parentProcessId: 3000,
            parentProcessName: "cmd.exe",
            parentProcessPath: "C:\\Windows\\System32\\cmd.exe",
            parentCommandLine: "cmd.exe");
    }

    private static SyntheticEndpointEvent CreateRegistryDiscovery()
    {
        return CreateProcessEvent(
            processId: 4400,
            processName: "reg.exe",
            processPath: "C:\\Windows\\System32\\reg.exe",
            commandLine: "reg.exe query HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\Run",
            parentProcessId: 3000,
            parentProcessName: "cmd.exe",
            parentProcessPath: "C:\\Windows\\System32\\cmd.exe",
            parentCommandLine: "cmd.exe");
    }

    private static SyntheticEndpointEvent CreateRundll32Url()
    {
        return CreateProcessEvent(
            processId: 4500,
            processName: "rundll32.exe",
            processPath: "C:\\Windows\\System32\\rundll32.exe",
            commandLine: "rundll32.exe url.dll,FileProtocolHandler https://example.com/SAFE_SAMPLE_ONLY",
            parentProcessId: 3000,
            parentProcessName: "cmd.exe",
            parentProcessPath: "C:\\Windows\\System32\\cmd.exe",
            parentCommandLine: "cmd.exe");
    }

    private static SyntheticEndpointEvent CreateServiceDiscovery()
    {
        return CreateProcessEvent(
            processId: 4600,
            processName: "sc.exe",
            processPath: "C:\\Windows\\System32\\sc.exe",
            commandLine: "sc.exe query type= service state= all",
            parentProcessId: 3000,
            parentProcessName: "cmd.exe",
            parentProcessPath: "C:\\Windows\\System32\\cmd.exe",
            parentCommandLine: "cmd.exe");
    }

    private static SyntheticEndpointEvent CreateProcessEvent(
        int processId,
        string processName,
        string processPath,
        string commandLine,
        int parentProcessId,
        string parentProcessName,
        string parentProcessPath,
        string parentCommandLine)
    {
        return new SyntheticEndpointEvent
        {
            Source = "Simulator",
            EventId = 1,
            EventType = 1,
            Hostname = "PURPLETRACE-LAB",
            UserName = "lab-user",
            ProcessId = processId,
            ProcessName = processName,
            ProcessPath = processPath,
            CommandLine = commandLine,
            ParentProcessId = parentProcessId,
            ParentProcessName = parentProcessName,
            ParentProcessPath = parentProcessPath,
            ParentCommandLine = parentCommandLine,
            DestinationIp = "",
            DestinationPort = 0,
            Protocol = "",
            AdditionalData = new Dictionary<string, string>
            {
                ["GeneratedBy"] = "PurpleTrace.Simulator",
                ["SafetyScope"] = "Synthetic telemetry only"
            }
        };
    }
}

public sealed class SyntheticEndpointEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    public string Source { get; set; } = "Simulator";
    public int EventId { get; set; } = 1;
    public int EventType { get; set; } = 1;
    public string Hostname { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string ProcessGuid { get; set; } = string.Empty;
    public int ProcessId { get; set; }
    public string ProcessName { get; set; } = string.Empty;
    public string ProcessPath { get; set; } = string.Empty;
    public string CommandLine { get; set; } = string.Empty;
    public string ParentProcessGuid { get; set; } = string.Empty;
    public int ParentProcessId { get; set; }
    public string ParentProcessName { get; set; } = string.Empty;
    public string ParentProcessPath { get; set; } = string.Empty;
    public string ParentCommandLine { get; set; } = string.Empty;
    public string DestinationIp { get; set; } = string.Empty;
    public int DestinationPort { get; set; }
    public string Protocol { get; set; } = string.Empty;
    public Dictionary<string, string> AdditionalData { get; set; } = new();
}
