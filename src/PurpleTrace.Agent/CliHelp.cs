namespace PurpleTrace.Agent;

public static class CliHelp
{
    public static void Print()
    {
        Console.WriteLine("PurpleTrace Agent");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  PurpleTrace.Agent --config config/purpletrace.sample.json");
        Console.WriteLine("  PurpleTrace.Agent --list-rules --rules rules");
        Console.WriteLine("  PurpleTrace.Agent --validate-rules --rules rules");
        Console.WriteLine("  PurpleTrace.Agent --source sample --rules rules --event samples/sample-powershell-event.json --out samples/alerts.local.json --report samples/report.local.md --csv samples/alerts.local.csv --html samples/report.local.html");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --config             JSON config file path");
        Console.WriteLine("  --list-rules         List detection rules and exit");
        Console.WriteLine("  --validate-rules     Validate detection rule files and exit");
        Console.WriteLine("  --source             Event source mode. Supported values: sample, sysmon");
        Console.WriteLine("  --rules              Detection rules directory");
        Console.WriteLine("  --event              Sample endpoint event JSON file");
        Console.WriteLine("  --out                JSON alert output path");
        Console.WriteLine("  --report             Markdown report output path");
        Console.WriteLine("  --csv                CSV alert output path");
        Console.WriteLine("  --html               HTML report output path");
        Console.WriteLine("  --min-severity       Minimum alert severity to export. Values: Critical, High, Medium, Low, Informational");
        Console.WriteLine("  --mitre-technique    Export only alerts matching a MITRE technique ID");
        Console.WriteLine("  --mitre              Short alias for --mitre-technique");
        Console.WriteLine("  --rule-id            Export only alerts matching a detection rule ID");
        Console.WriteLine("  --rule               Short alias for --rule-id");
        Console.WriteLine("  --max                Maximum Sysmon events to read");
        Console.WriteLine("  --help               Show help");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run --project src\\PurpleTrace.Agent -- --config config/purpletrace.sample.json");
        Console.WriteLine("  dotnet run --project src\\PurpleTrace.Agent -- --config config/purpletrace.sample.json --min-severity High");
        Console.WriteLine("  dotnet run --project src\\PurpleTrace.Agent -- --config config/purpletrace.sample.json --mitre-technique T1082");
        Console.WriteLine("  dotnet run --project src\\PurpleTrace.Agent -- --config config/purpletrace.sample.json --rule-id PT-RULE-003");
        Console.WriteLine("  dotnet run --project src\\PurpleTrace.Agent -- --list-rules --rules rules");
        Console.WriteLine("  dotnet run --project src\\PurpleTrace.Agent -- --validate-rules --rules rules");
    }
}
