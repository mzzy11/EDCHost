using System.Text.Json.Serialization;

namespace EdcHost.ViewerServers;

public interface IHostConfigurationFromServer : IMessage
{
    [JsonPropertyName("availableCameras")]
    public List<object> AvailableCameras { get; }

    [JsonPropertyName("availableSerialPorts")]
    public List<object> AvailableSerialPorts { get; }

    [JsonPropertyName("message")]
    public string Message { get; }
}