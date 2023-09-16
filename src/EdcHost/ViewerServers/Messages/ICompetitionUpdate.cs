using System.Text.Json.Serialization;

namespace EdcHost.ViewerServers.Messages;

public interface ICompetitionUpdate : IMessage
{
    List<object> Cameras { get; }
    List<object> Chunks { get; }
    List<object> Events { get; }
    List<object> Info { get; }
    List<object> Mines { get; }
    List<object> Players { get; }
}
