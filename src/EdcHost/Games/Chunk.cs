namespace EdcHost.Games;

public class Chunk : IChunk
{

    #region Public properties

    public int Height { get; }
    public bool IsVoid { get; }
    public bool CanPlaceBlock { get; }
    public IPosition<int> Position { get; }

    #endregion

    #region Public methods

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="height">Height of the chunk.</param>
    public Chunk(int height, IPosition<int> position)
    {
        Height = height;
        IsVoid = (height == 0);
        CanPlaceBlock = true;
        Position = position;
    }

    #endregion
}
