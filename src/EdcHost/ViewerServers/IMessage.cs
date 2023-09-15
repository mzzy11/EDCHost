using System.Text.Json.Serialization;

namespace EdcHost.ViewerServers;

/// <summary>
/// Store information of game status.
/// </summary>
public interface IMessage
{
    [JsonPropertyName("messageType")]
    public string MessageType { get; }

    public byte[] SerializeToUtf8Bytes();

    public string SerializeToString();
}