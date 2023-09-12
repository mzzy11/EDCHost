namespace EdcHost.Games;

public class Map : IMap
{

    #region Private fields

    private readonly Chunk[,] _chunks;

    #endregion

    #region Public methods

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="spawnPoints">Position of spawn points</param>
    public Map(List<Position<int>> spawnPoints)
    {
        _chunks = new Chunk[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (spawnPoints.Exists(position => position == new Position<int>(i, j)))
                {
                    _chunks[i, j] = new Chunk(1);
                }
                else
                {
                    _chunks[i, j] = new Chunk(0);
                }
            }
        }
        GenerateMine();
    }

    /// <summary>
    /// Get chunk at a certain position
    /// </summary>
    /// <param name="position">The position</param>
    /// <returns></returns>
    public IChunk GetChunkAt(IPosition<int> position)
    {
        return _chunks[position.X, position.Y];
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Generate mines
    /// </summary>
    private void GenerateMine()
    {
        //TODO: Check the rules of generating mines
    }

    #endregion
}
