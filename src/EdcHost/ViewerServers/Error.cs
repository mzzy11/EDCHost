using System.Text.Json;
using System.Text.Json.Serialization;

namespace EdcHost.ViewerServers;

public class Error : IError
{
    public string MessageType { get; }
    public int ErrorCode { get; }
    public string Message { get; }

    [JsonConstructor]
    public Error(string messageType, int errorCode, string message)
        => (MessageType, ErrorCode, Message) = (messageType, errorCode, message);

    public byte[] Serialize() => JsonSerializer.SerializeToUtf8Bytes(this);
}