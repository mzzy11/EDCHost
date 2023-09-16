using System.Text.Json;
using System.Text.Json.Serialization;

namespace EdcHost.ViewerServers.Messages;

public class CompetitionControlCommand : ICompetitionControlCommand
{
    // The message type of the command
    [JsonPropertyName("messageType")]
    public string MessageType { get; }
    // The token of the command
    [JsonPropertyName("token")]
    public string Token { get; }
    // The command to be executed
    [JsonPropertyName("command")]
    public string Command { get; }

    // Serializes the command into a byte array
    public byte[] SerializeToUtf8Bytes() => JsonSerializer.SerializeToUtf8Bytes(this);

    [JsonConstructor]
    public CompetitionControlCommand(string messageType, string token, string command)
        => (MessageType, Token, Command) = (messageType, token, command);

    public string SerializeToString() => JsonSerializer.Serialize(this);
}