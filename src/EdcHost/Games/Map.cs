namespace EdcHost.Games;

public class Map : IMap
{

    #region Public properties

    public List<IChunk> Chunks { get; }

    #endregion

    #region Public methods

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="chunks"></param>
    public Map(List<IChunk> chunks)
    {
        Chunks = chunks;
    }

    public IChunk GetChunkAt(IPosition<int> position)
    {
        return Chunks[8 * position.X + position.Y];
    }

    #endregion
}
