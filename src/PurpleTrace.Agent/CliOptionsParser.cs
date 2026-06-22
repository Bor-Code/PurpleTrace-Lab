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
        }

        return options;
    }
}
