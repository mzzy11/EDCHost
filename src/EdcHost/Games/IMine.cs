namespace EdcHost.Games;

public interface IMine
{
    public enum OreKindType
    {
        IronIngot,
        GoldIngot,
        Diamond,
    }

    public int GeneratedOreCount { get; }
    public int OreKind { get; }
    public IPosition<float> Position { get; }
}
