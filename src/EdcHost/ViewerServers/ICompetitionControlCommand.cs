using System.Text.Json.Serialization;

namespace EdcHost.ViewerServers;

public interface ICompetitionControlCommand : IMessage
{
    public string Token { get; }
    public string Command { get; }
}