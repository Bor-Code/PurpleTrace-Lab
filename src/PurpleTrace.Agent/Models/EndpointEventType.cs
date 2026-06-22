namespace PurpleTrace.Agent.Models;

public enum EndpointEventType
{
    Unknown = 0,
    ProcessCreate = 1,
    ProcessTerminate = 2,
    NetworkConnection = 3,
    FileCreate = 4,
    RegistryChange = 5,
    ImageLoad = 6
}
