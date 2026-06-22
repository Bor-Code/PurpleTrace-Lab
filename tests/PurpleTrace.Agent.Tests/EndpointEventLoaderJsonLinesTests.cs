using PurpleTrace.Agent.Collectors;

namespace PurpleTrace.Agent.Tests;

public sealed class EndpointEventLoaderJsonLinesTests
{
    [Fact]
    public void LoadManyFromJsonFile_ShouldLoadEventsFromJsonLinesFile()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), $"purpletrace-events-{Guid.NewGuid():N}.jsonl");

        try
        {
            File.WriteAllLines(outputPath, new[]
            {
                "{\"Source\":\"Sample\",\"EventId\":1,\"EventType\":1,\"Hostname\":\"BORAN\",\"UserName\":\"nonmr\",\"ProcessName\":\"powershell.exe\",\"CommandLine\":\"powershell.exe -NoProfile\",\"ParentProcessName\":\"cmd.exe\"}",
                "{\"Source\":\"Sample\",\"EventId\":1,\"EventType\":1,\"Hostname\":\"BORAN\",\"UserName\":\"nonmr\",\"ProcessName\":\"cmd.exe\",\"CommandLine\":\"cmd.exe /c whoami\",\"ParentProcessName\":\"explorer.exe\"}"
            });

            var loader = new EndpointEventLoader();

            var events = loader.LoadManyFromJsonFile(outputPath);

            Assert.Equal(2, events.Count);
            Assert.Equal("powershell.exe", events[0].ProcessName);
            Assert.Equal("cmd.exe", events[1].ProcessName);
        }
        finally
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
        }
    }

    [Fact]
    public void LoadManyFromJsonFile_ShouldIgnoreEmptyLinesInJsonLinesFile()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), $"purpletrace-events-{Guid.NewGuid():N}.jsonl");

        try
        {
            File.WriteAllLines(outputPath, new[]
            {
                "",
                "{\"Source\":\"Sample\",\"EventId\":1,\"EventType\":1,\"Hostname\":\"BORAN\",\"UserName\":\"nonmr\",\"ProcessName\":\"powershell.exe\",\"CommandLine\":\"powershell.exe -NoProfile\",\"ParentProcessName\":\"cmd.exe\"}",
                "   "
            });

            var loader = new EndpointEventLoader();

            var events = loader.LoadManyFromJsonFile(outputPath);

            Assert.Single(events);
            Assert.Equal("powershell.exe", events[0].ProcessName);
        }
        finally
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
        }
    }

    [Fact]
    public void LoadManyFromJsonFile_ShouldThrowFriendlyError_WhenJsonLinesFileHasInvalidLine()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), $"purpletrace-events-{Guid.NewGuid():N}.jsonl");

        try
        {
            File.WriteAllLines(outputPath, new[]
            {
                "{\"Source\":\"Sample\",\"EventId\":1,\"EventType\":1,\"ProcessName\":\"powershell.exe\"}",
                "{ invalid json"
            });

            var loader = new EndpointEventLoader();

            var exception = Assert.Throws<InvalidOperationException>(() => loader.LoadManyFromJsonFile(outputPath));

            Assert.Contains("Invalid JSONL event at line 2", exception.Message);
        }
        finally
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
        }
    }
}
