using System.Text.Json;
using System.Text.Json.Serialization;

namespace EdcHost.ViewerServers;

public class HostConfigurationFromServer : IHostConfigurationFromServer
{
    public string MessageType { get; }
    public List<object> AvailableCameras { get; }
    public List<object> AvailableSerialPorts { get; }
    public string Message { get; }

    [JsonConstructor]
    public HostConfigurationFromServer(
        string messageType, 
        List<object> availableCameras, 
        List<object> availableSerialPorts, 
        string message
    )  => (MessageType, AvailableCameras, AvailableSerialPorts, Message) 
        = (messageType, availableCameras, availableSerialPorts, message);
    
    public byte[] SerializeToUtf8Bytes() => JsonSerializer.SerializeToUtf8Bytes(this);

    public string SerializeToString() => JsonSerializer.Serialize(this);
}