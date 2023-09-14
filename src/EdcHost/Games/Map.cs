namespace EdcHost.Games;

/// <summary>
/// Map represents the map of the game.
/// </summary>
public class Map : IMap
{

    /// <summary>
    /// The list of chunks.
    /// </summary>
    public List<IChunk> Chunks { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    public Map()
    {
        Chunks = new();
        for (int i = 0; i < 64; i++)
        {
            Chunks.Add(new Chunk(0, new Position<int>(i / 8, i % 8)));
        }
    }

    /// <summary>
    /// Gets the chunk at the position.
    /// </summary>
    /// <param name="position">The position</param>
    /// <returns>The chunk</returns>
    public IChunk GetChunkAt(IPosition<int> position)
    {
        return Chunks[8 * position.X + position.Y];
    }

}
