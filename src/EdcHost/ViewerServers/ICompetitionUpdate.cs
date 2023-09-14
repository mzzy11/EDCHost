using System.Text.Json.Serialization;

namespace EdcHost.ViewerServers;

public interface ICompetitionUpdate : IMessage
{
    [JsonPropertyName("cameras")]
    List<object> Cameras { get; }

    [JsonPropertyName("chunks")]
    List<object> Chunks { get; }

    [JsonPropertyName("events")]
    List<object> Events { get; }

    [JsonPropertyName("info")]
    List<object> Info { get; }

    [JsonPropertyName("mines")]
    List<object> Mines { get; }

    [JsonPropertyName("players")]
    List<object> Players { get; }
}