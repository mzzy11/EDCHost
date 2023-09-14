using System.Text.Json.Serialization;

namespace EdcHost.ViewerServers;

public interface IError : IMessage
{
    [JsonPropertyName("errorCode")]
    public int ErrorCode { get; }

    [JsonPropertyName("message")]
    public string Message { get; }
}