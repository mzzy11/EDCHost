namespace EdcHost.Games;

public interface IMap
{
    public IChunk GetChunkAt(IPosition<int> position);
}
