using PurpleTrace.Agent.Models;

namespace PurpleTrace.Agent;

public static class CliOptionsParser
{
    public static CliOptions Parse(string[] args)
    {
        var options = new CliOptions();

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

            if (current.Equals("--list-rules", StringComparison.OrdinalIgnoreCase))
            {
                options.ListRules = true;
                continue;
            }

            if (current.Equals("--config", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                options.ConfigPath = args[i + 1];
                i++;
                continue;
            }

            if (current.Equals("--rules", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                options.RulesDirectory = args[i + 1];
                i++;
                continue;
            }

            if (current.Equals("--event", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                options.EventPath = args[i + 1];
                i++;
                continue;
            }

            if (current.Equals("--out", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                options.OutputPath = args[i + 1];
                i++;
                continue;
            }

            if (current.Equals("--report", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                options.ReportPath = args[i + 1];
                i++;
                continue;
            }

            if (current.Equals("--csv", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                options.CsvPath = args[i + 1];
                i++;
                continue;
            }

            if (current.Equals("--source", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                options.Source = args[i + 1];
                i++;
                continue;
            }

            if (current.Equals("--max", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                if (int.TryParse(args[i + 1], out var maxEvents))
                {
                    options.MaxEvents = maxEvents;
                }

                i++;
                continue;
            }
        }

        return options;
    }
}
