namespace EdcHost.Games;

/// <summary>
/// Map represents the map of the game.
/// </summary>
public interface IMap
{
    /// <summary>
    /// The list of chunks.
    /// </summary>
    public List<IChunk> Chunks { get; }

    /// <summary>
    /// Gets the chunk at the position.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public IChunk GetChunkAt(IPosition<int> position);
}
