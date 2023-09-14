using System.Text.Json.Serialization;

namespace EdcHost.ViewerServers;

public interface ICompetitionControlCommand : IMessage
{
    [JsonPropertyName("token")]
    public string Token { get; }

    [JsonPropertyName("command")]
    public string Command { get; }
}