using System.Text.Json;
using System.Text.Json.Serialization;

namespace EdcHost.ViewerServers;

public class CompetitionUpdate : ICompetitionUpdate
{
    public string MessageType { get; }
    public List<object> Cameras { get; }
    public List<object> Chunks { get; }
    public List<object> Events { get; }
    public List<object> Info { get; }
    public List<object> Mines { get; }
    public List<object> Players { get; }
    

    [JsonConstructor]
    public CompetitionUpdate (
        string messageType,
        List<object> cameras,
        List<object> chunks,
        List<object> events,
        List<object> info,
        List<object> mines,
        List<object> players
    )  => (MessageType, Cameras, Chunks, Events, Info, Mines, Players) 
        = (messageType, cameras, chunks, events, info, mines, players);

    public byte[] SerializeToUtf8Bytes() => JsonSerializer.SerializeToUtf8Bytes(this);

    public string SerializeToString() => JsonSerializer.Serialize(this);
}