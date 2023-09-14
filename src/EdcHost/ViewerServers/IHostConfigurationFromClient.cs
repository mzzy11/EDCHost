using System.Text.Json.Serialization;

namespace EdcHost.ViewerServers;

public interface IHostConfigurationFromClient : IMessage
{
    [JsonPropertyName("token")]
    public string Token { get; }
    
    [JsonPropertyName("players")]
    public List<object> Players { get; }
}