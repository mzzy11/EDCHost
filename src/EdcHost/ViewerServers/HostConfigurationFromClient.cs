using System.Data.SqlTypes;
using System.Diagnostics.Contracts;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using EdcHost.Games;

namespace EdcHost.ViewerServers;

public class HostConfigurationFromClient : IHostConfigurationFromClient
{
    // The message type of the command
    public string MessageType { get; private set; }
    // The token of the command
    public string Token { get; private set; }
    // The command to be executed
    public List<object> Players{ get; private set; }

    [JsonConstructor]
    public HostConfigurationFromClient(string messageType, string token, List<object> players) 
        => (MessageType, Token, Players) = (messageType, token, players);
    
    public byte[] Serialize() => JsonSerializer.SerializeToUtf8Bytes(this);
}