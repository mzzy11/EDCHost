using System.Text.Json.Serialization;

namespace EdcHost.ViewerServers;

public interface IHostConfigurationFromClient : IMessage
{
    public string Token { get; }
    public List<object> Players { get; }
}