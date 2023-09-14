using System.Text.Json;
using System.Text.Json.Serialization;

namespace EdcHost.ViewerServers;

public class CompetitionControlCommand : ICompetitionControlCommand
{
    // The message type of the command
    public string MessageType { get; }
    // The token of the command
    public string Token { get; }
    // The command to be executed
    public string Command { get; }

    // Serializes the command into a byte array
    public byte[] Serialize() => JsonSerializer.SerializeToUtf8Bytes(this);

    [JsonConstructor]
    public CompetitionControlCommand(string messageType, string token, string command) 
        => (MessageType, Token, Command) = (messageType, token, command);
}